using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using BitContainer.Contracts.V1;
using BitContainer.Contracts.V1.Storage;
using BitContainer.DataAccess.DataProviders;
using BitContainer.DataAccess.Models;

namespace BitContainer.StorageService.Managers
{
    public static class CLoadsManager
    {
        // TODO : This class need refactoring (Andrey Gurin)
        // TODO : Implement connections management (Andrey Gurin)
        // TODO : Implement connections securtiy (Andrey Gurin)
        // TODO : Add more complex reaction to the exception (Andrey Gurin)  
        // TODO : Big files (read/write stream to db)

        private static readonly CTransmissionEndPointContract UploadEndPoint =
            CTransmissionEndPointContract.Create(IPAddress.Loopback, 20000);
        private static readonly CTransmissionEndPointContract DownloadEndPoint =
            CTransmissionEndPointContract.Create(IPAddress.Loopback, 20001);

        public static CTransmissionEndPointContract GetEndPointToUpload()
        {
            return UploadEndPoint;
        }

        public static CTransmissionEndPointContract GetEndPointToDownload()
        {
            return DownloadEndPoint;
        }

        static CLoadsManager()
        {
            InitDownload();
            InitUpload();
        }
        
        public static void InitDownload()
        {
            Task.Run(async () =>
            {
                TcpListener listener = new TcpListener(DownloadEndPoint.Address, DownloadEndPoint.Port);
                var storage = new CStorageProvider();
                try
                {
                    listener.Start();
                    while (true)
                    {
                        using TcpClient uploader = await listener.AcceptTcpClientAsync();
                        using NetworkStream networkStream = uploader.GetStream();

                        byte[] typeBytes = await ReadFromNetwork(networkStream, sizeof(Int32));
                        Int32 numericType = BitConverter.ToInt32(typeBytes);
                        EEntityTypeContract type = (EEntityTypeContract) numericType;
                        
                        byte[] idBytes = await ReadFromNetwork(networkStream, _guidSize);
                        Guid id = new Guid(Encoding.UTF8.GetString(idBytes));

                        byte[] ownerIdBytes = await ReadFromNetwork(networkStream, _guidSize);
                        Guid ownerId = new Guid(Encoding.UTF8.GetString(ownerIdBytes));

                        switch (type)
                        {
                            case EEntityTypeContract.File:

                                await LoadFile(storage, networkStream, id);

                                break;
                            case EEntityTypeContract.Directory:

                                await LoadDir(storage, networkStream, id);

                                break;
                            default:
                                throw new ArgumentOutOfRangeException("Unexpected entity type.");
                        }
                    }
                }
                catch (Exception)
                {
                    listener.Stop();
                    throw;
                }
            });
        }

        public static async Task LoadFile(CStorageProvider storage, NetworkStream networkStream, Guid id)
        {
             byte[] fileData = storage.StorageEntities.GetAllFileData(id);
             await networkStream.WriteAsync(fileData, 0, fileData.Length);
        }

        public static async Task LoadDir(CStorageProvider storage, NetworkStream networkStream, Guid id)
        {
            Dictionary<Int32, List<IStorageEntity>> children = storage.StorageEntities.GetAllChildren(id);

            MemoryStream memory = new MemoryStream();
            using (ZipArchive archive = new ZipArchive(memory, ZipArchiveMode.Create))
            {
                Int32 level = 1;
            
                IStorageEntity rootDir =  children[level].Single();
                Dictionary<Guid, String> paths = new Dictionary<Guid, String>();
                paths[rootDir.Id] = $@"{rootDir.Name}\";
                archive.CreateEntry(paths[rootDir.Id]);

                level++;

                while (children.ContainsKey(level))
                {
                    List<IStorageEntity> entity = children[level];

                    foreach (var storageEntity in entity)
                    {
                        switch (storageEntity)
                        {
                            case CFile file:
                                byte[] fileData = storage.StorageEntities.GetAllFileData(file.Id);
                                ZipArchiveEntry entry = archive.CreateEntry($@"{paths[file.ParentId]}{file.Name}");

                                using (var stream = entry.Open())
                                {
                                    await stream.WriteAsync(fileData, 0, fileData.Length);
                                }

                                break;
                            case CDirectory dir:
                                String path = $@"{paths[dir.ParentId]}{dir.Name}\";
                                archive.CreateEntry(path);
                                paths[dir.Id] = path;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("Unexpected entity type.");
                        }
                    }
                    level++;
                }
            }

            Byte[] archiveBytes = memory.ToArray();
            await networkStream.WriteAsync(archiveBytes, 0, archiveBytes.Length);
        }

        private static readonly int _guidSize = Guid.Empty.ToString().Length;

        public static void InitUpload()
        {
            Task.Run(async () =>
            {
                TcpListener listener = new TcpListener(UploadEndPoint.Address, UploadEndPoint.Port);
                var storage = new CStorageProvider();
                try
                {
                    listener.Start();
                    while (true)
                    {
                        using TcpClient uploader = await listener.AcceptTcpClientAsync();
                        using NetworkStream networkStream = uploader.GetStream();

                        byte[] nameSizeBytes = await ReadFromNetwork(networkStream, sizeof(Int32));
                        Int32 nameSize = BitConverter.ToInt32(nameSizeBytes);

                        byte[] fileNameBytes = await ReadFromNetwork(networkStream, nameSize);
                        String fileName = Encoding.UTF8.GetString(fileNameBytes);

                        byte[] parentIdBytes = await ReadFromNetwork(networkStream, _guidSize);
                        Guid parentId = new Guid(Encoding.UTF8.GetString(parentIdBytes));

                        byte[] ownerIdBytes = await ReadFromNetwork(networkStream, _guidSize);
                        Guid ownerId = new Guid(Encoding.UTF8.GetString(ownerIdBytes));
                        
                        Guid realOwnerId = storage.Shares.GetStorageEntityOwner(parentId);
                        if (realOwnerId != Guid.Empty)
                        {
                            ownerId = realOwnerId;
                        }
                        
                        byte[] fileSizeBytes = await ReadFromNetwork(networkStream, sizeof(Int32));
                        Int32 fileSize = BitConverter.ToInt32(fileSizeBytes);

                        byte[] fileData = await ReadFromNetwork(networkStream, fileSize);

                        CFile newfle = storage.StorageEntities.AddFile(parentId, ownerId, fileName, fileData);

                        byte[] fileIdBytes = Encoding.UTF8.GetBytes(newfle.Id.ToString());
                        await networkStream.WriteAsync(fileIdBytes, 0, fileIdBytes.Length);
                    }
                }
                catch (Exception)
                {
                    listener.Stop();
                    throw;
                }
            });
        }

        private static async Task<Byte[]> ReadFromNetwork(NetworkStream stream, Int32 size, Int32 blockSize = 10000)
        {
            Byte[] data = new byte[size];
            Int32 hasRead = 0;

            while (hasRead < data.Length)
            {
                Int32 remainder = data.Length - hasRead;
                Boolean remainderLessThanBlock = remainder < blockSize;
                Int32 block = remainderLessThanBlock ? remainder : blockSize;
                hasRead += await stream.ReadAsync(data, hasRead, block);
            }

            return data;
        }
    }
}
