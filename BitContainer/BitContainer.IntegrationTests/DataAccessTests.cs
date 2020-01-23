using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BitContainer.DataAccess;
using BitContainer.DataAccess.DataProviders;
using BitContainer.DataAccess.Helpers;
using BitContainer.DataAccess.Models.Shares;
using BitContainer.DataAccess.Models.StorageEntities;
using BitContainer.DataAccess.Scripts;
using BitContainer.Shared.Auth;
using BitContainer.Shared.Models;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace BitContainer.IntegrationTests
{
    [TestFixture]
    public class DataAccessTests
    {
        private String _sqlDbConnection = "Server=.\\SQLExpress;Integrated security=true;";

        private CSqlDbHelper _storageDbHelper;
        private CStorageProvider _storage;
        private CUsersProvider _users;

        private CUserId _firstUserId;
        private CUserId _secondUserId;

        private const String FirstUserName = "TestBitContainerUser1";
        private const String SecondUserName = "TestBitContainerUser2";
        private const String PasswordString = "1234";

        private const String DirName1 = "FirstDir";
        private const String DirName2 = "SecondDir";
        private const String FileName1 = "FirstFile";

        private readonly Byte[] _fileData1 = {1, 1, 1, 1};
        private readonly CStorageEntityId _rootId = CStorageEntityId.RootId;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            String storageDbName = $"TEST_{DbNames.StorageDbName}";
            String authDbName = $"TEST_{DbNames.AuthDbName}";
            T4StorageDbInitScript storageScript = new T4StorageDbInitScript(storageDbName);
            T4AuthDbInitScript initScript = new T4AuthDbInitScript(authDbName);

            _storageDbHelper = new CSqlDbHelper(_sqlDbConnection, storageScript, Mock.Of<ILogger<CSqlDbHelper>>());
            CSqlDbHelper authDbHelper = new CSqlDbHelper(_sqlDbConnection, initScript, Mock.Of<ILogger<CSqlDbHelper>>());
            
            _storage = new CStorageProvider(_storageDbHelper);
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
                IStorageEntity dir1 = _storage.Entities.GetStorageEntity(_rootId, userOne.Id, DirName1);
                
                IStorageEntity dir3 = _storage.Entities.GetStorageEntity(_rootId, userTwo.Id, DirName1);
                IStorageEntity file = _storage.Entities.GetStorageEntity(_rootId, userTwo.Id, FileName1);

                if (dir1 != null)
                {
                    IStorageEntity dir2 = _storage.Entities.GetStorageEntity(dir1.Id, userOne.Id, DirName2);

                    if (dir2 != null)
                        _storage.Shares.DeleteShare(dir2.Id, userTwo.Id);

                    _storage.Entities.DeleteEntity(dir1.Id);
                }
                    
                if (dir3 != null)
                    _storage.Entities.DeleteEntity(dir3.Id);
                if (file != null)
                    _storage.Entities.DeleteEntity(file.Id);

                _users.RemoveUser(userOne.Id);
                _users.RemoveUser(userTwo.Id);

                _storageDbHelper.ExecuteTransaction((command) =>
                {
                    _storage.Stats.RemoveStats(command, userOne.Id);
                    _storage.Stats.RemoveStats(command, userTwo.Id);
                });
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
            
            _storageDbHelper.ExecuteTransaction((command) =>
            {
                _storage.Stats.AddNewStats(command, userOne.Id);
                _storage.Stats.AddNewStats(command, userTwo.Id);
            });

            CStats stats1 = _storage.Stats.GetStats(userOne.Id);
            Assert.That(stats1, Is.Not.Null);
            CStats stats2 = _storage.Stats.GetStats(userTwo.Id);
            Assert.That(stats2, Is.Not.Null);

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
            CUserId ownerId = _firstUserId;

            String dirName3 = "ThirdDir";
            String dirName4 = "FourthDir";

            String fileName2 = "SecondFile";
            Byte[] fileData2 = {2, 2, 2, 2};
            
            CSharableEntity e1 = _storage.Entities.AddDir(CDirectory.Root, ownerId, DirName1);
            Assert.That(e1, Is.Not.Null);
            IStorageEntity dir1 = e1.AccessWrapper.Entity;
            Assert.That(dir1, Is.Not.Null);
            Assert.AreEqual(dir1.Name, DirName1);
            Assert.AreEqual(dir1.ParentId, _rootId);
            Assert.AreEqual(dir1.OwnerId, ownerId);

            CSharableEntity e2 = _storage.Entities.AddDir(dir1, ownerId, DirName2);
            Assert.That(e2, Is.Not.Null);
            IStorageEntity dir2 = e2.AccessWrapper.Entity;
            Assert.That(dir2, Is.Not.Null);
            Assert.AreEqual(dir2.Name, DirName2);
            Assert.AreEqual(dir2.ParentId, dir1.Id);

            CSharableEntity e3 = _storage.Entities.AddDir(dir2, ownerId, dirName3);
            Assert.That(e3, Is.Not.Null);
            IStorageEntity dir3 = e3.AccessWrapper.Entity;
            Assert.That(dir3, Is.Not.Null);
            Assert.AreEqual(dir3.Name, dirName3);
            Assert.AreEqual(dir3.ParentId, dir2.Id);

            CSharableEntity ef1;
            using (var stream = new MemoryStream(_fileData1))
            {
                ef1 = _storage.Entities.AddFileAsync(stream, dir3, ownerId, FileName1, _fileData1.LongLength).Result;
            }

            Assert.That(ef1, Is.Not.Null);

            IStorageEntity file1 = ef1.AccessWrapper.Entity;

            Assert.That(file1, Is.Not.Null);
            Assert.AreEqual(file1.Name, FileName1);
            Assert.AreEqual(file1.ParentId, dir3.Id);
            Assert.AreEqual(file1.OwnerId, ownerId);

            _storageDbHelper.ExecuteTransaction((command) =>
            {
                using var stream = _storage.Entities.GetFileStream(command, file1.Id, FileAccess.Read);
                Assert.AreEqual(stream.Length, _fileData1.Length);
                byte[] test = new byte[_fileData1.Length];
                stream.Read(test, 0, test.Length);
                Assert.AreEqual(test, _fileData1);
            });

            CSharableEntity e4 = _storage.Entities.AddDir(dir3, ownerId, dirName4);
            IStorageEntity dir4 = e4.AccessWrapper.Entity;

            Assert.That(dir4, Is.Not.Null);
            Assert.AreEqual(dir4.Name, dirName4);
            Assert.AreEqual(dir4.ParentId, dir3.Id);

            CSharableEntity ef2;
            using (MemoryStream stream = new MemoryStream(fileData2))
            {
                ef2 = _storage.Entities.AddFileAsync(stream, dir4, ownerId, fileName2, fileData2.LongLength).Result;
            }

            IStorageEntity file2 = ef2.AccessWrapper.Entity;

            Assert.That(file2, Is.Not.Null);
            Assert.AreEqual(file2.Name, fileName2);
            Assert.AreEqual(file2.ParentId, dir4.Id);

            CStats stats = _storage.Stats.GetStats(_firstUserId);
            Assert.AreEqual(stats.DirsCount, 4);
            Assert.AreEqual(stats.FilesCount, 2);
            Assert.AreEqual(stats.StorageSize, _fileData1.Length + fileData2.Length);
            
            String newName = "NewName";
            _storage.Entities.RenameEntity(file1.Id, newName);

            IStorageEntity renamedFile = _storage.Entities.GetStorageEntity(file1.Id);
            Assert.That(renamedFile, Is.Not.Null);
            Assert.AreEqual(renamedFile.Name, newName);

            _storage.Entities.RenameEntity(dir3.Id, newName);

            IStorageEntity renamedDir = _storage.Entities.GetStorageEntity(dir3.Id);
            Assert.That(renamedDir, Is.Not.Null);
            Assert.AreEqual(renamedDir.Name, newName);

            _storage.Entities.DeleteEntity(file1.Id);
            IStorageEntity f = _storage.Entities.GetStorageEntity(file1.Id);
            Assert.That(f, Is.Null);

            _storage.Entities.DeleteEntity(dir1.Id);
            IStorageEntity d1 = _storage.Entities.GetStorageEntity(dir1.Id);
            IStorageEntity d2 = _storage.Entities.GetStorageEntity(dir2.Id);
            IStorageEntity d3 = _storage.Entities.GetStorageEntity(dir3.Id);
            IStorageEntity d4 = _storage.Entities.GetStorageEntity(dir4.Id);
            IStorageEntity f2 = _storage.Entities.GetStorageEntity(file2.Id);
            
            Assert.That(d1, Is.Null);
            Assert.That(d2, Is.Null);
            Assert.That(d3, Is.Null);
            Assert.That(d4, Is.Null);
            Assert.That(f2, Is.Null);
        }

        [Test, Order(3)]
        public void ShareTests()
        {
            CUserId ownerId = _firstUserId;
            CUserId friendId = _secondUserId;
            
            _storage.Entities.AddDir(CDirectory.Root, ownerId, DirName1);
            IStorageEntity dir1 = _storage.Entities.GetStorageEntity(_rootId, ownerId, DirName1);

            _storage.Entities.AddDir(CDirectory.Root, friendId, DirName1);
            IStorageEntity friendDir = _storage.Entities.GetStorageEntity(_rootId, friendId, DirName1);

            CSharableEntity ef1;
            using (var stream = new MemoryStream(_fileData1))
            {
                ef1 = _storage.Entities.AddFileAsync(stream, CDirectory.Root, friendId, FileName1, _fileData1.LongLength).Result;
            }

            IStorageEntity friendFile = ef1.AccessWrapper.Entity;
            CSharableEntity e1 = _storage.Entities.AddDir(dir1, ownerId, DirName2);
            IStorageEntity dir2 = e1.AccessWrapper.Entity;

            CSharableEntity ef2;
            using (var stream = new MemoryStream(_fileData1))
            {
                ef2 = _storage.Entities.AddFileAsync(stream, dir2, ownerId, FileName1, _fileData1.LongLength).Result;
            }

            IStorageEntity file = ef2.AccessWrapper.Entity;

            _storage.Shares.SaveShare(dir2.Id, friendId, EAccessType.Read);
            Assert.DoesNotThrow(() => _storage.Validator.EntityExists(dir2.Id).HasReadAccess(friendId));

            List<CSharableEntity> friendRoot = _storage.Entities.GetOwnerChildren(_rootId, friendId);
            Assert.That(friendRoot.Count, Is.EqualTo(2));
            Assert.That(friendRoot.Select(elem => elem.AccessWrapper.Entity.Id.ToGuid()), 
                Is.EquivalentTo(new []{friendDir.Id.ToGuid(), friendFile.Id.ToGuid()}));

            List<CSharableEntity> sharedRoot = _storage.Entities.GetSharedChildren(_rootId, friendId);
            Assert.That(sharedRoot.Count, Is.EqualTo(1));
            Assert.That(sharedRoot[0].AccessWrapper.Entity.Id, Is.EqualTo(dir2.Id));
            Assert.That(sharedRoot[0].AccessWrapper.AcesssType, Is.EqualTo(EAccessType.Read));

            List<CSharableEntity> dirChildren = _storage.Entities.GetSharedChildren(dir2.Id, friendId);
            Assert.That(dirChildren.Count, Is.EqualTo(1));
            Assert.That(dirChildren[0].AccessWrapper.Entity.Id, Is.EqualTo(file.Id));

            _storage.Shares.SaveShare(dir2.Id, friendId, EAccessType.Write);

            Assert.DoesNotThrow(() => _storage.Validator.EntityExists(dir2.Id).HasWriteAccess(friendId));
            Assert.DoesNotThrow(() => _storage.Validator.EntityExists(file.Id).HasWriteAccess(friendId));
            
            _storage.Shares.DeleteShare(dir2.Id, friendId);
            List<CShare> shares = _storage.Shares.GetShares(dir2.Id);
            Assert.That(shares, Is.Empty);

        }
    }
}