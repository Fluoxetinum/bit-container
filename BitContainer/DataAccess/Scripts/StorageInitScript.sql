USE [master]
EXEC master.dbo.xp_create_subdir "C:\BitContainerData"

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'BITCONTAINER_STORAGE_DB')
BEGIN
	CREATE DATABASE BITCONTAINER_STORAGE_DB;
	ALTER DATABASE BITCONTAINER_STORAGE_DB ADD FILEGROUP [BitContainer-Storage-FileGroup] CONTAINS FILESTREAM;
	ALTER DATABASE BITCONTAINER_STORAGE_DB ADD FILE (NAME = N'BitContainer-Storage', FILENAME = N'C:\BitContainerData\Storage') TO FILEGROUP [BitContainer-Storage-FileGroup];
END 
GO

USE BITCONTAINER_STORAGE_DB;
EXEC sp_configure filestream_access_level, 2
RECONFIGURE

IF NOT EXISTS (SELECT * FROM sysobjects where name='StorageEntities')
BEGIN
	CREATE TABLE StorageEntities (
		ID UNIQUEIDENTIFIER ROWGUIDCOL PRIMARY KEY DEFAULT(newid()),
		ParentID UNIQUEIDENTIFIER,
		OwnerID UNIQUEIDENTIFIER NOT NULL,
		Name NVARCHAR(300) NOT NULL,
		Created DATETIME NOT NULL DEFAULT(getdate()),
		Data VARBINARY(MAX) FILESTREAM,
		Size INT,

		FOREIGN KEY(ParentID) REFERENCES StorageEntities(ID)
	);
END

IF NOT EXISTS (SELECT * FROM sysobjects where name='AccessTypes')
BEGIN
	CREATE TABLE AccessTypes (
		ID INT PRIMARY KEY,
		Name VARCHAR(10) NOT NULL
	);

	INSERT INTO AccessTypes VALUES (0, 'NONE'), (1, 'READ'), (2, 'WRITE');
END

IF NOT EXISTS (SELECT * FROM sysobjects where name='Shares')
BEGIN
	CREATE TABLE Shares (
		UserApprovedID UNIQUEIDENTIFIER NOT NULL,
		AccessTypeID INT NOT NULL,
		EntityID UNIQUEIDENTIFIER NOT NULL,

		FOREIGN KEY(EntityID) REFERENCES StorageEntities(ID),
		FOREIGN KEY(AccessTypeID) REFERENCES AccessTypes(ID)
	);
END

IF NOT EXISTS (SELECT * FROM sysobjects where name='Stats')
BEGIN
	CREATE TABLE Stats (
		UserID UNIQUEIDENTIFIER PRIMARY KEY,
		FilesCount INT NOT NULL,
		DirectoriesCount INT NOT NULL,
		StorageSize INT NOT NULL
	);
END

----------------------------------------------------------- 

IF NOT EXISTS (SELECT * FROM sys.objects WHERE   object_id = OBJECT_ID(N'sp_GetAllDirChildren') AND type IN ( N'P', N'PC' ))
BEGIN
	DROP [dbo].[sp_GetAllDirChildren] 
END
GO

CREATE PROCEDURE [dbo].[sp_GetAllDirChildren] 
	@DirID UNIQUEIDENTIFIER 
AS
BEGIN
	
	WITH entitiesReqursive AS 
			( 
				SELECT ID, ParentID, Size, 1 AS Level 
				FROM StorageEntities 
				WHERE ID = @DirID 
				UNION ALL 
				SELECT entites.ID, entites.ParentID, entites.Size, entitiesReqursive.Level+1 AS Level 
				FROM StorageEntities AS entites 
				INNER JOIN entitiesReqursive 
				ON entitiesReqursive.ID = entites.ParentID 
			) 
			SELECT ID, Size, Level 
			FROM entitiesReqursive 
			ORDER BY Level;

END
GO

----------------------------------------------------------- 

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'sp_GetOwnerDirChildren') AND type IN ( N'P', N'PC' ))
BEGIN
	DROP [dbo].[sp_GetOwnerDirChildren] 
END
GO

CREATE PROCEDURE [dbo].[sp_GetOwnerDirChildren] 
	@OwnerID UNIQUEIDENTIFIER,
	@ParentID UNIQUEIDENTIFIER = null
AS
BEGIN
	
	SELECT ID, ParentID, OwnerID, Name, Created, Size, 
	(CASE WHEN Shares.EntityID IS NULL THEN 0 ELSE 1 END) AS IsShared
	FROM StorageEntities 
	LEFT OUTER JOIN Shares ON StorageEntities.ID = Shares.EntityID
	WHERE ((@ParentID IS NULL AND ParentID IS NULL) OR (@ParentID IS NOT NULL AND ParentID = @ParentID))
	AND OwnerID = @OwnerID;

END
GO

----------------------------------------------------------- 

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'sp_GetShareById') AND type IN ( N'P', N'PC' ))
BEGIN
	DROP [dbo].[sp_GetShareById] 
END
GO

ALTER PROCEDURE [dbo].[sp_GetShareById] 
	@PersonID UNIQUEIDENTIFIER,
	@EntityID UNIQUEIDENTIFIER
AS
BEGIN
	
WITH entitiesReqursive AS 
            ( 
				SELECT ID, ParentID, 1 AS Level 
				FROM StorageEntities 
				WHERE ID = @EntityID 
				UNION ALL 
				SELECT entites.ID, entites.ParentID, entitiesReqursive.Level+1 AS Level 
				FROM StorageEntities AS entites 
				INNER JOIN entitiesReqursive 
				ON entitiesReqursive.ParentID = entites.ID 
            ) 
            SELECT TOP(1) UserApprovedID, AccessTypes.Name AS AccessType, EntityID AS StorageEntityId, Level 
            FROM Shares JOIN entitiesReqursive ON Shares.EntityID = entitiesReqursive.ID 
			JOIN AccessTypes ON Shares.AccessTypeID = AccessTypes.ID
            WHERE UserApprovedID = @PersonID 
            ORDER BY Level;
END
GO

----------------------------------------------------------- 

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'sp_GetSharedDirChildren') AND type IN ( N'P', N'PC' ))
BEGIN
	DROP [dbo].[sp_GetSharedDirChildren] 
END
GO

ALTER PROCEDURE [dbo].[sp_GetSharedDirChildren] 
	@ParentID UNIQUEIDENTIFIER,
	@PersonID UNIQUEIDENTIFIER,
	@ParentAccess VARCHAR(50)
AS
BEGIN
	
SELECT DISTINCT StorageEntities.ID, ParentID, OwnerID, StorageEntities.Name, Created, Size, 

(CASE WHEN Shares.UserApprovedID IS NULL THEN @ParentAccess
	 ELSE AccessTypes.Name 
	 END) AS AccessType 

FROM StorageEntities 
LEFT OUTER JOIN Shares ON StorageEntities.ID = Shares.EntityID 
LEFT OUTER JOIN AccessTypes ON AccessTypes.ID = Shares.AccessTypeID
WHERE ParentID = @ParentID AND (Shares.UserApprovedID = @PersonID OR Shares.UserApprovedID IS NULL)

END
GO

----------------------------------------------------------- 

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'sp_GetSharedRootChildren') AND type IN ( N'P', N'PC' ))
BEGIN
	DROP [dbo].[sp_GetSharedRootChildren] 
END
GO

ALTER PROCEDURE [dbo].[sp_GetSharedRootChildren] 
	@PersonID UNIQUEIDENTIFIER
AS
BEGIN
	
SELECT SE.ID, SE.ParentID, SE.OwnerID, SE.Name, SE.Created, T.Name AS AccessType
                                      FROM Shares AS S
                                      JOIN StorageEntities AS SE ON SE.ID = S.EntityID
                                      JOIN AccessTypes AS T ON T.ID = S.AccessTypeID
                                      WHERE S.UserApprovedID = @PersonID
                                      AND NOT EXISTS 
									  (SELECT S2.EntityID FROM Shares AS S2 WHERE S2.UserApprovedID = @PersonID AND S2.EntityID IN (SELECT ParentID FROM GetParents(SE.ID)));

END
GO

----------------------------------------------------------- 

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'sp_RemoveDir') AND type IN ( N'P', N'PC' ))
BEGIN
	DROP [dbo].[sp_RemoveDir] 
END
GO

CREATE PROCEDURE [dbo].[sp_RemoveDir] 
	@DirID UNIQUEIDENTIFIER 
AS
BEGIN
	
	DECLARE @children TABLE(ID UNIQUEIDENTIFIER, Size INT, Level INT);

	INSERT @children EXEC sp_GetAllDirChildren @DirID;

	DELETE FROM StorageEntities WHERE ID IN (SELECT ID FROM @children ORDER BY Level ASC OFFSET 0 ROWS);

END
GO

-----------------------------------

DROP FUNCTION [dbo].[GetParents];
GO

CREATE FUNCTION [dbo].[GetParents]
(	
	@DirID UNIQUEIDENTIFIER
)
RETURNS TABLE 
AS
RETURN 
(
	WITH entitiesReqursive AS 
            ( 
            SELECT ID, ParentID, 1 AS Level 
            FROM StorageEntities 
            WHERE ID = @DirID 
            UNION ALL 
            SELECT entities.ID, entities.ParentID, entitiesReqursive.Level+1 AS Level 
            FROM StorageEntities AS entities 
            INNER JOIN entitiesReqursive 
            ON entitiesReqursive.ParentID = entities.ID 
            ) 
	SELECT ParentID FROM entitiesReqursive
)

