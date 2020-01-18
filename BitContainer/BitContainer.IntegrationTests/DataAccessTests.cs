using System;
using System.Collections.Generic;
using System.Linq;
using BitContainer.DataAccess;
using BitContainer.DataAccess.DataProviders;
using BitContainer.DataAccess.Models;
using BitContainer.DataAccess.Queries.Base;
using BitContainer.DataAccess.Scripts;
using BitContainer.Shared.Auth;
using NUnit.Framework;

namespace BitContainer.IntegrationTests
{
    [TestFixture]
    public class DataAccessTests
    {
        private String _sqlDbConnection = "Server=.\\SQLExpress;User Id=UserWithMasterRights;Password=1234;";

        private CStorageProvider _storage;
        private CUsersProvider _users;

        private Guid _firstUserId;
        private Guid _secondUserId;

        private const String FirstUserName = "TestBitContainerUser1";
        private const String SecondUserName = "TestBitContainerUser2";
        private const String PasswordString = "1234";

        private const String DirName1 = "FirstDir";
        private const String DirName2 = "SecondDir";
        private const String FileName1 = "FirstFile";

        private readonly Byte[] _fileData1 = {1, 1, 1, 1};
        private readonly Guid _rootId = Guid.Empty;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            String storageDbName = $"TEST_{DbNames.StorageDbName}";
            String authDbName = $"TEST_{DbNames.AuthDbName}";
            T4StorageDbInitScript storageScript = new T4StorageDbInitScript(storageDbName);
            T4AuthDbInitScript initScript = new T4AuthDbInitScript(authDbName);

            CSqlDbHelper storageDbHelper = new CSqlDbHelper(_sqlDbConnection, storageScript);
            CSqlDbHelper authDbHelper = new CSqlDbHelper(_sqlDbConnection, initScript);
            
            _storage = new CStorageProvider(storageDbHelper);
            _users = new CUsersProvider(authDbHelper);

            OneTimeTearDown();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            CUser userOne = _users.GetUserWithName(FirstUserName);
            CUser userTwo = _users.GetUserWithName(SecondUserName);
            
            if (userTwo != null && userOne != null)
            {
                CDirectory dir1 = _storage.StorageEntities.GetDir(_rootId, userOne.Id, DirName1);
                
                CDirectory dir3 = _storage.StorageEntities.GetDir(_rootId,userTwo.Id, DirName1);
                CFile file = _storage.StorageEntities.GetFile(_rootId, userTwo.Id, FileName1);

                if (dir1 != null)
                {
                    CDirectory dir2 = _storage.StorageEntities.GetDir(dir1.Id, userOne.Id, DirName2);

                    if (dir2 != null)
                        _storage.Shares.DeleteStorgeEntityShare(userTwo.Id, dir2.Id);

                    _storage.StorageEntities.DeleteDir(dir1.Id);
                }
                    
                if (dir3 != null)
                    _storage.StorageEntities.DeleteDir(dir3.Id);
                if (file != null)
                    _storage.StorageEntities.DeleteFile(file.Id);

                _users.RemoveUser(userOne.Id);
                _users.RemoveUser(userTwo.Id);
                _storage.Stats.RemoveStats(userOne.Id);
                _storage.Stats.RemoveStats(userTwo.Id);
            }
        }


        [Test, Order(1)]
        public void UserTests()
        {
            Byte[] firstCreatedSalt = CryptoHelper.GenerateSalt();
            Byte[] secondCreatedSalt = CryptoHelper.GenerateSalt();

            Byte[] password = CryptoHelper.GenerateHashWithStaticSalt(PasswordString);
            Byte[] firstHashedPassword = CryptoHelper.GenerateHash(password, firstCreatedSalt);
            Byte[] secondHashedPassword = CryptoHelper.GenerateHash(password, secondCreatedSalt);

            Int32 resultOne = _users.AddNewUser(FirstUserName, firstHashedPassword, firstCreatedSalt);
            Assert.That(resultOne, Is.EqualTo(1));
            
            Int32 resultTwo = _users.AddNewUser(SecondUserName, secondHashedPassword, secondCreatedSalt);
            Assert.That(resultTwo, Is.EqualTo(1));

            CUser userOne = _users.GetUserWithName(FirstUserName);
            CUser userTwo = _users.GetUserWithName(SecondUserName);

            Assert.That(userOne, Is.Not.Null);
            Assert.That(userTwo, Is.Not.Null);
            Assert.That(userOne.Name, Is.EqualTo(FirstUserName));
            Assert.That(userTwo.Name, Is.EqualTo(SecondUserName));
            
            Int32 statsOne = _storage.Stats.AddNewStats(userOne.Id);
            Assert.That(statsOne, Is.EqualTo(1));
            Int32 statsTwo = _storage.Stats.AddNewStats(userTwo.Id);
            Assert.That(statsTwo, Is.EqualTo(1));

            String incorrectPasswordString = "99999";
            Byte[] incorrectPassword = CryptoHelper.GenerateHashWithStaticSalt(incorrectPasswordString);

            Byte[] firstUserSalt = _users.GetSalt(FirstUserName);

            Assert.AreEqual(firstUserSalt, firstCreatedSalt);

            Byte[] incorrectPasswordHash = CryptoHelper.GenerateHash(incorrectPassword, firstUserSalt);
            CUser loggedInUser = _users.GetUserWithCredentials(FirstUserName, incorrectPasswordHash);

            Assert.That(loggedInUser, Is.Null);

            String incorrectUserName = "IncredibleTestUser";
            loggedInUser = _users.GetUserWithCredentials(incorrectUserName, firstHashedPassword);

            Assert.That(loggedInUser, Is.Null);

            loggedInUser = _users.GetUserWithCredentials(FirstUserName, firstHashedPassword);

            Assert.That(loggedInUser, Is.Not.Null);
            Assert.That(loggedInUser.Id, Is.EqualTo(userOne.Id));

            _firstUserId = userOne.Id;
            _secondUserId = userTwo.Id;
        }

        [Test, Order(2)]
        public void StorageTests()
        {
            Guid ownerId = _firstUserId;

            String dirName3 = "ThirdDir";
            String dirName4 = "FourthDir";

            String fileName2 = "SecondFile";
            Byte[] fileData2 = {2, 2, 2, 2};
            
            CDirectory dir1 = _storage.StorageEntities.AddDir(parentId:_rootId, ownerId, DirName1);

            Assert.That(dir1, Is.Not.Null);
            Assert.AreEqual(dir1.Name, DirName1);
            Assert.AreEqual(dir1.ParentId, _rootId);
            Assert.AreEqual(dir1.OwnerId, ownerId);

            CDirectory dir2 = _storage.StorageEntities.AddDir(parentId:dir1.Id, ownerId, DirName2);

            Assert.That(dir2, Is.Not.Null);
            Assert.AreEqual(dir2.Name, DirName2);
            Assert.AreEqual(dir2.ParentId, dir1.Id);

            CDirectory dir3 = _storage.StorageEntities.AddDir(parentId:dir2.Id, ownerId, dirName3);

            Assert.That(dir3, Is.Not.Null);
            Assert.AreEqual(dir3.Name, dirName3);
            Assert.AreEqual(dir3.ParentId, dir2.Id);

            CFile file1 = _storage.StorageEntities.AddFile(parentId:dir3.Id, ownerId, FileName1, _fileData1);

            Assert.That(file1, Is.Not.Null);
            Assert.AreEqual(file1.Name, FileName1);
            Assert.AreEqual(file1.ParentId, dir3.Id);
            Assert.AreEqual(file1.OwnerId, ownerId);

            Byte[] data = _storage.StorageEntities.GetAllFileData(file1.Id);
            Assert.AreEqual(data, _fileData1);

            CDirectory dir4 = _storage.StorageEntities.AddDir(parentId:dir3.Id, ownerId, dirName4);

            Assert.That(dir4, Is.Not.Null);
            Assert.AreEqual(dir4.Name, dirName4);
            Assert.AreEqual(dir4.ParentId, dir3.Id);

            CFile file2 =_storage.StorageEntities.AddFile(parentId:dir4.Id, ownerId, fileName2, fileData2);

            Assert.That(file2, Is.Not.Null);
            Assert.AreEqual(file2.Name, fileName2);
            Assert.AreEqual(file2.ParentId, dir4.Id);

            CUserStats stats = _storage.Stats.GetStats(_firstUserId);
            Assert.AreEqual(stats.DirsCount, 4);
            Assert.AreEqual(stats.FilesCount, 2);
            Assert.AreEqual(stats.StorageSize, _fileData1.Length + fileData2.Length);
            
            String newName = "NewName";
            Int32 result7 = _storage.StorageEntities.RenameEntity(file1.Id, newName);
            Assert.That(result7, Is.EqualTo(1));

            CFile renamedFile = _storage.StorageEntities.GetFile(file1.Id);
            Assert.That(renamedFile, Is.Not.Null);
            Assert.AreEqual(renamedFile.Name, newName);

            Int32 result8 = _storage.StorageEntities.RenameEntity(dir3.Id, newName);
            Assert.That(result8, Is.EqualTo(1));

            CDirectory renamedDir = _storage.StorageEntities.GetDir(dir3.Id);
            Assert.That(renamedDir, Is.Not.Null);
            Assert.AreEqual(renamedDir.Name, newName);

            Int32 result9 = _storage.StorageEntities.DeleteFile(file1.Id);
            CFile f = _storage.StorageEntities.GetFile(file1.Id);
            Assert.That(result9, Is.EqualTo(1));
            Assert.That(f, Is.Null);

            Int32 result10 = _storage.StorageEntities.DeleteDir(dir1.Id);
            Assert.That(result10, Is.EqualTo(5));
        }

        [Test, Order(3)]
        public void ShareTests()
        {
            Guid ownerId = _firstUserId;
            Guid friendId = _secondUserId;
            
            _storage.StorageEntities.AddDir(parentId:_rootId, ownerId, DirName1);
            CDirectory dir1 = _storage.StorageEntities.GetDir(_rootId, ownerId, DirName1);

            _storage.StorageEntities.AddDir(parentId: _rootId, friendId, DirName1);
            CDirectory friendDir = _storage.StorageEntities.GetDir(_rootId, friendId, DirName1);

            CFile friendFile = _storage.StorageEntities.AddFile(_rootId, friendId, FileName1, _fileData1);
            CDirectory dir2 = _storage.StorageEntities.AddDir(parentId:dir1.Id, ownerId, DirName2);

            CFile file = _storage.StorageEntities.AddFile(parentId: dir2.Id, ownerId, FileName1, _fileData1);

            _storage.Shares.AddStorageEntityShare(friendId, ERestrictedAccessType.Read, dir2.Id);

            CShare dirShare = _storage.Shares.GetStorageEntityShare(friendId, dir2.Id);
            Assert.That(dirShare.RestrictedAccessType, Is.EqualTo(ERestrictedAccessType.Read));

            List<COwnStorageEntity> friendRoot = _storage.StorageEntities.GetOwnerChildren(_rootId, friendId);
            Assert.That(friendRoot.Count, Is.EqualTo(2));
            Assert.That(friendRoot.Select(elem => elem.Entity.Id), Is.EquivalentTo(new Guid[]{friendDir.Id, friendFile.Id}));

            List<CRestrictedStorageEntity> sharedRoot = _storage.StorageEntities.GetSharedChildren(_rootId, friendId);
            Assert.That(sharedRoot.Count, Is.EqualTo(1));
            Assert.That(sharedRoot[0].Entity.Id, Is.EqualTo(dir2.Id));
            Assert.That(sharedRoot[0].RestrictedAccess, Is.EqualTo(ERestrictedAccessType.Read));

            List<CRestrictedStorageEntity> dirChildren = _storage.StorageEntities.GetSharedChildren(dir2.Id, friendId);
            Assert.That(dirChildren.Count, Is.EqualTo(1));
            Assert.That(dirChildren[0].Entity.Id, Is.EqualTo(file.Id));

            _storage.Shares.UpdateStorageEntityShare(friendId, ERestrictedAccessType.Write, dir2.Id);
            dirShare = _storage.Shares.GetStorageEntityShare(friendId, dir2.Id);
            Assert.That(dirShare.RestrictedAccessType, Is.EqualTo(ERestrictedAccessType.Write));

            ERestrictedAccessType restrictedAccessType = _storage.Shares.CheckStorageEntityAccess(file.Id, friendId);
            Assert.That(restrictedAccessType, Is.EqualTo(ERestrictedAccessType.Write));

            Int32 result1 = _storage.Shares.DeleteStorgeEntityShare(friendId, dir2.Id);
            Assert.That(result1, Is.EqualTo(1));
        }
    }
}