using System;
using System.Collections.Generic;
using System.Text;
using BitContainer.DataAccess;
using BitContainer.DataAccess.DataProviders;
using BitContainer.DataAccess.Models;
using BitContainer.DataAccess.Queries;
using BitContainer.Shared;
using BitContainer.Shared.Auth;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace BitContainer.Tests
{
    [TestFixture]
    public class DataAccessTests
    {

        private String _sqlTestDbConnection = "Server=MKR0014\\SQLEXPRESS,1433;" +
                                                              "Database=BitContainerDb;" +
                                                              "User Id=DefaultUser;" +
                                                              "Password=pass1234!;";
        private CStorageProvider Storage;
        private CUsersProvider Users;

        private String firstUserName = "TestBitContainerUser1";
        private String secondUserName = "TestBitContainerUser2";
        private String passwordString = "1234";

        private Guid FirstUserId;
        private Guid SecondUserId;

        private Guid rootId = Guid.Empty;
        private String dirName1 = "FirstDir";
        private String dirName2 = "SecondDir";

        private String fileName1 = "FirstFile";
        private Byte[] fileData1 = {1, 1, 1, 1};

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            //CDbHelper.Init(_sqlTestDbConnection);
            Storage = new CStorageProvider();
            Users = new CUsersProvider();

            OneTimeTearDown();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            CUser userOne = Users.GetUserWithName(firstUserName);
            CUser userTwo = Users.GetUserWithName(secondUserName);
            
            if (userTwo != null && userOne != null)
            {
                CDirectory dir1 = Storage.StorageEntities.GetDir(rootId, userOne.Id, dirName1);
                
                CDirectory dir3 = Storage.StorageEntities.GetDir(rootId,userTwo.Id, dirName1);
                CFile file = Storage.StorageEntities.GetFile(rootId, userTwo.Id, fileName1);

                if (dir1 != null)
                {
                    CDirectory dir2 = Storage.StorageEntities.GetDir(dir1.Id, userOne.Id, dirName2);

                    if (dir2 != null)
                        Storage.Shares.DeleteStorgeEntityShare(userTwo.Id, dir2.Id);

                    Storage.StorageEntities.DeleteDir(dir1.Id);
                }
                    
                if (dir3 != null)
                    Storage.StorageEntities.DeleteDir(dir3.Id);
                if (file != null)
                    Storage.StorageEntities.DeleteFile(file.Id);

                Users.RemoveUser(userOne.Id);
                Users.RemoveUser(userTwo.Id);
            }
        }


        [Test, Order(1)]
        public void UserTests()
        {
            Byte[] firstCreatedSalt = CryptoHelper.GenerateSalt();
            Byte[] secondCreatedSalt = CryptoHelper.GenerateSalt();

            Byte[] password = CryptoHelper.GenerateHashWithStaticSalt(passwordString);
            Byte[] firstHashedPassword = CryptoHelper.GenerateHash(password, firstCreatedSalt);
            Byte[] secondHashedPassword = CryptoHelper.GenerateHash(password, secondCreatedSalt);

            Int32 resultOne = Users.AddNewUser(firstUserName, firstHashedPassword, firstCreatedSalt);
            Assert.That(resultOne, Is.EqualTo(1));

            Int32 resultTwo = Users.AddNewUser(secondUserName, secondHashedPassword, secondCreatedSalt);
            Assert.That(resultTwo, Is.EqualTo(1));

            CUser userOne = Users.GetUserWithName(firstUserName);
            CUser userTwo = Users.GetUserWithName(secondUserName);

            Assert.That(userOne, Is.Not.Null);
            Assert.That(userTwo, Is.Not.Null);
            Assert.That(userOne.Name, Is.EqualTo(firstUserName));
            Assert.That(userTwo.Name, Is.EqualTo(secondUserName));
            
            String incorrectPasswordString = "99999";
            Byte[] incorrectPassword = CryptoHelper.GenerateHashWithStaticSalt(incorrectPasswordString);

            Byte[] firstUserSalt = Users.GetSalt(firstUserName);

            Assert.AreEqual(firstUserSalt, firstCreatedSalt);

            Byte[] incorrectPasswordHash = CryptoHelper.GenerateHash(incorrectPassword, firstUserSalt);
            CUser loggedInUser = Users.GetUserWithCredentials(firstUserName, incorrectPasswordHash);

            Assert.That(loggedInUser, Is.Null);

            String incorrectUserName = "IncredibleTestUser";
            loggedInUser = Users.GetUserWithCredentials(incorrectUserName, firstHashedPassword);

            Assert.That(loggedInUser, Is.Null);

            loggedInUser = Users.GetUserWithCredentials(firstUserName, firstHashedPassword);

            Assert.That(loggedInUser, Is.Not.Null);
            Assert.That(loggedInUser.Id, Is.EqualTo(userOne.Id));

            FirstUserId = userOne.Id;
            SecondUserId = userTwo.Id;
        }

        [Test, Order(2)]
        public void StorageTests()
        {
            Guid ownerId = FirstUserId;

            String dirName3 = "ThirdDir";
            String dirName4 = "FourthDir";

            String fileName2 = "SecondFile";
            Byte[] fileData2 = {2, 2, 2, 2};
            
            CDirectory dir1 = Storage.StorageEntities.AddDir(parentId:rootId, ownerId, dirName1);

            Assert.That(dir1, Is.Not.Null);
            Assert.AreEqual(dir1.Name, dirName1);
            Assert.AreEqual(dir1.ParentId, rootId);
            Assert.AreEqual(dir1.OwnerId, ownerId);

            CDirectory dir2 = Storage.StorageEntities.AddDir(parentId:dir1.Id, ownerId, dirName2);

            Assert.That(dir2, Is.Not.Null);
            Assert.AreEqual(dir2.Name, dirName2);
            Assert.AreEqual(dir2.ParentId, dir1.Id);

            CDirectory dir3 = Storage.StorageEntities.AddDir(parentId:dir2.Id, ownerId, dirName3);

            Assert.That(dir3, Is.Not.Null);
            Assert.AreEqual(dir3.Name, dirName3);
            Assert.AreEqual(dir3.ParentId, dir2.Id);

            CFile file1 = Storage.StorageEntities.AddFile(parentId:dir3.Id, ownerId, fileName1, fileData1);

            Assert.That(file1, Is.Not.Null);
            Assert.AreEqual(file1.Name, fileName1);
            Assert.AreEqual(file1.ParentId, dir3.Id);
            Assert.AreEqual(file1.OwnerId, ownerId);

            Byte[] data = Storage.StorageEntities.GetAllFileData(file1.Id);
            Assert.AreEqual(data, fileData1);

            CDirectory dir4 = Storage.StorageEntities.AddDir(parentId:dir3.Id, ownerId, dirName4);

            Assert.That(dir4, Is.Not.Null);
            Assert.AreEqual(dir4.Name, dirName4);
            Assert.AreEqual(dir4.ParentId, dir3.Id);

            CFile file2 =Storage.StorageEntities.AddFile(parentId:dir4.Id, ownerId, fileName2, fileData2);

            Assert.That(file2, Is.Not.Null);
            Assert.AreEqual(file2.Name, fileName2);
            Assert.AreEqual(file2.ParentId, dir4.Id);


            //CUserStats stats = Users.GetStats(FirstUserId);
            //Assert.AreEqual(stats.DirsCount, 4);
            //Assert.AreEqual(stats.FilesCount, 2);
            //Assert.AreEqual(stats.StorageSize, fileData1.Length + fileData2.Length);


            String newName = "NewName";
            Int32 result7 = Storage.StorageEntities.RenameEntity(file1.Id, newName);
            Assert.That(result7, Is.EqualTo(1));

            CFile renamedFile = Storage.StorageEntities.GetFile(file1.Id);
            Assert.That(renamedFile, Is.Not.Null);
            Assert.AreEqual(renamedFile.Name, newName);

            Int32 result8 = Storage.StorageEntities.RenameEntity(dir3.Id, newName);
            Assert.That(result8, Is.EqualTo(1));

            CDirectory renamedDir = Storage.StorageEntities.GetDir(dir3.Id);
            Assert.That(renamedDir, Is.Not.Null);
            Assert.AreEqual(renamedDir.Name, newName);

            Int32 result9 = Storage.StorageEntities.DeleteFile(file1.Id);
            CFile f = Storage.StorageEntities.GetFile(file1.Id);
            Assert.That(result9, Is.EqualTo(1));
            Assert.That(f, Is.Null);

            Int32 result10 = Storage.StorageEntities.DeleteDir(dir1.Id);
            Assert.That(result10, Is.EqualTo(5));
        }

        [Test, Order(3)]
        public void ShareTests()
        {
            Guid ownerId = FirstUserId;
            Guid friendId = SecondUserId;
            
            Storage.StorageEntities.AddDir(parentId:rootId, ownerId, dirName1);
            CDirectory dir1 = Storage.StorageEntities.GetDir(rootId, ownerId, dirName1);

            Storage.StorageEntities.AddDir(parentId: rootId, friendId, dirName1);
            CDirectory friendDir = Storage.StorageEntities.GetDir(rootId, friendId, dirName1);

            CFile friendFile = Storage.StorageEntities.AddFile(rootId, friendId, fileName1, fileData1);
            CDirectory dir2 = Storage.StorageEntities.AddDir(parentId:dir1.Id, ownerId, dirName2);

            CFile file = Storage.StorageEntities.AddFile(parentId: dir2.Id, ownerId, fileName1, fileData1);

            Storage.Shares.AddStorageEntityShare(friendId, ERestrictedAccessType.Read, dir2.Id);

            CShare dirShare = Storage.Shares.GetStorageEntityShare(friendId, dir2.Id);
            Assert.That(dirShare.RestrictedAccessType, Is.EqualTo(ERestrictedAccessType.Read));

            List<COwnStorageEntity> friendRoot = Storage.StorageEntities.GetOwnerChildren(rootId, friendId);
            Assert.That(friendRoot.Count, Is.EqualTo(1));
            Assert.That(friendRoot[0].Entity.Id, Is.EqualTo(friendDir.Id));

            List<COwnStorageEntity> frientRootFiles = Storage.StorageEntities.GetOwnerChildren(rootId, friendId);
            Assert.That(frientRootFiles.Count, Is.EqualTo(1));
            Assert.That(frientRootFiles[0].Entity.Id, Is.EqualTo(friendFile.Id));
            
            List<CRestrictedStorageEntity> sharedRoot = Storage.StorageEntities.GetSharedChildren(rootId, friendId);
            Assert.That(sharedRoot.Count, Is.EqualTo(1));
            Assert.That(sharedRoot[0].Entity.Id, Is.EqualTo(dir2.Id));
            Assert.That(sharedRoot[0].RestrictedAccess, Is.EqualTo(ERestrictedAccessType.Read));

            List<CRestrictedStorageEntity> dirChildren = Storage.StorageEntities.GetSharedChildren(dir2.Id, friendId);
            Assert.That(dirChildren.Count, Is.EqualTo(1));
            Assert.That(dirChildren[0].Entity.Id, Is.EqualTo(file.Id));

            Storage.Shares.UpdateStorageEntityShare(friendId, ERestrictedAccessType.Write, dir2.Id);
            dirShare = Storage.Shares.GetStorageEntityShare(friendId, dir2.Id);
            Assert.That(dirShare.RestrictedAccessType, Is.EqualTo(ERestrictedAccessType.Write));

            ERestrictedAccessType restrictedAccessType = Storage.Shares.CheckStorageEntityAccess(file.Id, friendId);
            Assert.That(restrictedAccessType, Is.EqualTo(ERestrictedAccessType.Write));

            Int32 result1 = Storage.Shares.DeleteStorgeEntityShare(friendId, dir2.Id);
            Assert.That(result1, Is.EqualTo(1));
        }
    }
}
