USE [master]
CREATE DATABASE BitContainerDb;
GO

USE BitContainerDb;
EXEC sp_configure filestream_access_level, 2
RECONFIGURE


USE [master]
GO
ALTER DATABASE BitContainerDb ADD FILEGROUP [BitContainerFiles] CONTAINS FILESTREAM
GO
ALTER DATABASE BitContainerDb ADD FILE ( NAME = N'FilesData', FILENAME = N'C:\BCData\FilesData' ) TO FILEGROUP [BitContainerFiles]
GO

-- Generated ->
-------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------------------------

USE [BitContainerDb]
GO
/****** Object:  Table [dbo].[Directories]    Script Date: 12/2/2019 1:51:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Directories](
	[ID] [uniqueidentifier] NOT NULL,
	[ParentID] [uniqueidentifier] NULL,
	[OwnerID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Created] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UniqueDirectoryName] UNIQUE NONCLUSTERED 
(
	[ParentID] ASC,
	[Name] ASC,
	[OwnerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  UserDefinedFunction [dbo].[GetDirParents]    Script Date: 12/2/2019 1:51:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[GetDirParents]
(	
	@DirID UNIQUEIDENTIFIER
)
RETURNS TABLE 
AS
RETURN 
(

	WITH dirsReqursive AS 
            ( 
            SELECT ID, ParentID, 1 AS Level 
            FROM Directories 
            WHERE ID = @DirID 
            UNION ALL 
            SELECT dirs.ID, dirs.ParentID, dirsReqursive.Level+1 AS Level 
            FROM Directories AS dirs 
            INNER JOIN dirsReqursive 
            ON dirsReqursive.ParentID = dirs.ID 
            WHERE dirs.ID != (CAST(0x0 AS UNIQUEIDENTIFIER))
            ) 
	SELECT ParentID FROM dirsReqursive
)
GO
/****** Object:  Table [dbo].[Files]    Script Date: 12/2/2019 1:51:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Files](
	[ID] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[ParentID] [uniqueidentifier] NULL,
	[OwnerID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Created] [datetime] NULL,
	[Data] [varbinary](max) FILESTREAM  NULL,
	[Size] [int] NOT NULL,
	[LastModified] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY] FILESTREAM_ON [BitContainerFiles],
 CONSTRAINT [UniqueFileName] UNIQUE NONCLUSTERED 
(
	[ParentID] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] FILESTREAM_ON [BitContainerFiles]
GO
/****** Object:  UserDefinedFunction [dbo].[GetFileParents]    Script Date: 12/2/2019 1:51:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[GetFileParents]
(	
	@FileID UNIQUEIDENTIFIER
)
RETURNS TABLE 
AS
RETURN 
(

	WITH dirsReqursive AS 
            ( 
            SELECT ID, ParentID, 1 AS Level 
            FROM Files 
            WHERE ID = @FileID 
            UNION ALL 
            SELECT dirs.ID, dirs.ParentID, dirsReqursive.Level+1 AS Level 
            FROM Directories AS dirs 
            INNER JOIN dirsReqursive 
            ON dirsReqursive.ParentID = dirs.ID 
            WHERE dirs.ID != (CAST(0x0 AS UNIQUEIDENTIFIER))
            ) 
	SELECT ParentID FROM dirsReqursive
)
GO
/****** Object:  Table [dbo].[AccessTypes]    Script Date: 12/2/2019 1:51:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AccessTypes](
	[ID] [int] NOT NULL,
	[Name] [nvarchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Shares]    Script Date: 12/2/2019 1:51:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Shares](
	[ID] [uniqueidentifier] NOT NULL,
	[PersonApprovedID] [uniqueidentifier] NOT NULL,
	[AccessTypeID] [int] NOT NULL,
	[DirectoryID] [uniqueidentifier] NULL,
	[FileID] [uniqueidentifier] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Stats]    Script Date: 12/2/2019 1:51:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Stats](
	[ID] [uniqueidentifier] NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[FilesCount] [int] NOT NULL,
	[DirectoriesCount] [int] NOT NULL,
	[StorageSize] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 12/2/2019 1:51:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[ID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](200) NULL,
	[PasswordHash] [nvarchar](200) NOT NULL,
	[Salt] [nvarchar](200) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Directories] ADD  DEFAULT (newid()) FOR [ID]
GO
ALTER TABLE [dbo].[Directories] ADD  DEFAULT (CONVERT([uniqueidentifier],0x00)) FOR [ParentID]
GO
ALTER TABLE [dbo].[Directories] ADD  DEFAULT (getdate()) FOR [Created]
GO
ALTER TABLE [dbo].[Files] ADD  DEFAULT (newid()) FOR [ID]
GO
ALTER TABLE [dbo].[Files] ADD  DEFAULT (CONVERT([uniqueidentifier],0x00)) FOR [ParentID]
GO
ALTER TABLE [dbo].[Files] ADD  DEFAULT (getdate()) FOR [Created]
GO
ALTER TABLE [dbo].[Files] ADD  DEFAULT (getdate()) FOR [LastModified]
GO
ALTER TABLE [dbo].[Shares] ADD  DEFAULT (newid()) FOR [ID]
GO
ALTER TABLE [dbo].[Stats] ADD  DEFAULT (newid()) FOR [ID]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (newid()) FOR [ID]
GO
ALTER TABLE [dbo].[Directories]  WITH CHECK ADD FOREIGN KEY([OwnerID])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[Directories]  WITH CHECK ADD FOREIGN KEY([ParentID])
REFERENCES [dbo].[Directories] ([ID])
GO
ALTER TABLE [dbo].[Files]  WITH CHECK ADD FOREIGN KEY([OwnerID])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[Files]  WITH CHECK ADD FOREIGN KEY([ParentID])
REFERENCES [dbo].[Directories] ([ID])
GO
ALTER TABLE [dbo].[Shares]  WITH CHECK ADD FOREIGN KEY([AccessTypeID])
REFERENCES [dbo].[AccessTypes] ([ID])
GO
ALTER TABLE [dbo].[Shares]  WITH CHECK ADD FOREIGN KEY([DirectoryID])
REFERENCES [dbo].[Directories] ([ID])
GO
ALTER TABLE [dbo].[Shares]  WITH CHECK ADD FOREIGN KEY([FileID])
REFERENCES [dbo].[Files] ([ID])
GO
ALTER TABLE [dbo].[Shares]  WITH CHECK ADD FOREIGN KEY([PersonApprovedID])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[Stats]  WITH CHECK ADD FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([ID])
GO
ALTER TABLE [dbo].[Files]  WITH CHECK ADD CHECK  (([Size]>=(0)))
GO
ALTER TABLE [dbo].[Shares]  WITH CHECK ADD  CONSTRAINT [DirectoryOrFile] CHECK  (([FileID] IS NULL AND [DirectoryID] IS NOT NULL OR [FileID] IS NOT NULL AND [DirectoryID] IS NULL))
GO
ALTER TABLE [dbo].[Shares] CHECK CONSTRAINT [DirectoryOrFile]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetDirShareById]    Script Date: 12/2/2019 1:51:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetDirShareById] 
	@PersonID UNIQUEIDENTIFIER,
	@DirID UNIQUEIDENTIFIER
AS
BEGIN
	
WITH dirsReqursive AS 
            ( 
            SELECT ID, ParentID, 1 AS Level 
            FROM Directories 
            WHERE ID = @DirID 
            UNION ALL 
            SELECT dirs.ID, dirs.ParentID, dirsReqursive.Level+1 AS Level 
            FROM Directories AS dirs 
            INNER JOIN dirsReqursive 
            ON dirsReqursive.ParentID = dirs.ID 
            WHERE dirs.ID != (CAST(0x0 AS UNIQUEIDENTIFIER))
            ) 
            SELECT TOP(1) Shares.ID, PersonApprovedID, AccessTypeID, DirectoryID AS StorageEntityId, Level 
            FROM Shares JOIN dirsReqursive ON Shares.DirectoryID = dirsReqursive.ID 
            WHERE PersonApprovedID = @PersonID 
            ORDER BY Level;
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetFileShareById]    Script Date: 12/2/2019 1:51:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetFileShareById] 
	@PersonID UNIQUEIDENTIFIER,
	@FileID UNIQUEIDENTIFIER
AS
BEGIN
	
IF (EXISTS (SELECT ID FROM Shares WHERE FileID = @FileID))
BEGIN
	SELECT ID, PersonApprovedID, AccessTypeID, FileID AS StorageEntityId FROM Shares WHERE FileID = @FileID
END
ELSE
BEGIN
	WITH dirsReqursive AS 
				( 
				SELECT ID, ParentID, 1 AS Level 
				FROM Files 
				WHERE ID = @FileID 
				UNION ALL 
				SELECT dirs.ID, dirs.ParentID, dirsReqursive.Level+1 AS Level 
				FROM Directories AS dirs 
				INNER JOIN dirsReqursive 
				ON dirsReqursive.ParentID = dirs.ID 
				WHERE dirs.ID != (CAST(0x0 AS UNIQUEIDENTIFIER))
				) 
				SELECT TOP(1) Shares.ID, PersonApprovedID, AccessTypeID, DirectoryID AS StorageEntityId, Level 
				FROM Shares JOIN dirsReqursive ON Shares.DirectoryID = dirsReqursive.ID
				WHERE PersonApprovedID = @PersonID 
				ORDER BY Level;
END

END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetOwnerChildrenDirs]    Script Date: 12/2/2019 1:51:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetOwnerChildrenDirs] 
	@ParentID UNIQUEIDENTIFIER,
	@OwnerID UNIQUEIDENTIFIER
AS
BEGIN
	
SELECT Directories.ID, ParentID, OwnerID, Name, Created, 'OWNERONLY' AS AccessType FROM Directories 
LEFT OUTER JOIN Shares ON Directories.ID = Shares.DirectoryID
WHERE ParentID = @ParentID 
AND Directories.ID != '00000000-0000-0000-0000-000000000000' 
AND OwnerID = @OwnerID
AND Shares.DirectoryID IS NULL
UNION
SELECT DISTINCT Directories.ID, ParentID, OwnerID, Name, Created, 'SHAREDWITHOTHERS' AS AccessType FROM Directories 
JOIN Shares ON Directories.ID = Shares.DirectoryID
WHERE ParentID = @ParentID 
AND Directories.ID != '00000000-0000-0000-0000-000000000000' 
AND OwnerID = @OwnerID

END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetOwnerChildrenFiles]    Script Date: 12/2/2019 1:51:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetOwnerChildrenFiles] 
	@ParentID UNIQUEIDENTIFIER,
	@OwnerID UNIQUEIDENTIFIER
AS
BEGIN
	
SELECT Files.ID, ParentID, OwnerID, Name, Created, Size, LastModified, 'OWNERONLY' AS AccessType FROM Files 
LEFT OUTER JOIN Shares ON Files.ID = Shares.FileID
WHERE ParentID = @ParentID
AND Shares.FileID IS NULL
AND OwnerID = @OwnerID
UNION
SELECT DISTINCT Files.ID, ParentID, OwnerID, Name, Created, Size, LastModified, 'SHAREDWITHOTHERS' AS AccessType FROM Files 
JOIN Shares ON Files.ID = Shares.FileID
WHERE ParentID = @ParentID
AND OwnerID = @OwnerID

END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetSharedChildrenDirs]    Script Date: 12/2/2019 1:51:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetSharedChildrenDirs] 
	@ParentID UNIQUEIDENTIFIER,
	@PersonID UNIQUEIDENTIFIER,
	@ParentAccess VARCHAR(50)
AS
BEGIN
	
SELECT Directories.ID, ParentID, OwnerID, Name, Created, @ParentAccess AS AccessType FROM Directories 
WHERE ParentID = @ParentID AND 
(NOT EXISTS (SELECT ID FROM Shares WHERE Shares.DirectoryID = Directories.ID AND Shares.PersonApprovedID = @PersonID))
UNION
SELECT DISTINCT Directories.ID, ParentID, OwnerID, Directories.Name, Created, AccessTypes.Name AS AccessType FROM Directories 
JOIN Shares ON Directories.ID = Shares.DirectoryID 
JOIN AccessTypes ON AccessTypes.ID = Shares.AccessTypeID
WHERE ParentID = @ParentID AND Shares.PersonApprovedID = @PersonID

END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetSharedChildrenFiles]    Script Date: 12/2/2019 1:51:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetSharedChildrenFiles] 
	@ParentID UNIQUEIDENTIFIER,
	@PersonID UNIQUEIDENTIFIER,
	@ParentAccess VARCHAR(50)
AS
BEGIN
	
SELECT Files.ID, ParentID, OwnerID, Name, Created, Size, LastModified, @ParentAccess AS AccessType FROM Files 
WHERE ParentID = @ParentID AND 
(NOT EXISTS (SELECT ID FROM Shares WHERE Shares.FileID = Files.ID AND Shares.PersonApprovedID = @PersonID))
UNION
SELECT DISTINCT Files.ID, ParentID, OwnerID, Files.Name, Created, Size, LastModified, AccessTypes.Name AS AccessType FROM Files 
JOIN Shares ON Files.ID = Shares.FileID
JOIN AccessTypes ON AccessTypes.ID = Shares.AccessTypeID
WHERE ParentID = @ParentID AND Shares.PersonApprovedID = @PersonID

END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetSharedRootChildrenDirs]    Script Date: 12/2/2019 1:51:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetSharedRootChildrenDirs] 
	@PersonID UNIQUEIDENTIFIER
AS
BEGIN
	
SELECT D.ID, D.ParentID, D.OwnerID, D.Name, D.Created, T.Name AS AccessType
                                      FROM Shares AS S
                                      JOIN Directories AS D ON D.ID = S.DirectoryID
                                      JOIN AccessTypes AS T ON T.ID = S.AccessTypeID
                                      WHERE S.PersonApprovedID = @PersonID
                                      AND NOT EXISTS 
									  (SELECT S2.ID FROM Shares AS S2 WHERE S2.PersonApprovedID = @PersonID AND S2.ID IN (SELECT ParentID FROM GetDirParents(D.ID)));

END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetSharedRootChildrenFiles]    Script Date: 12/2/2019 1:51:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetSharedRootChildrenFiles] 
	@PersonID UNIQUEIDENTIFIER
AS
BEGIN
	
SELECT F.ID, F.ParentID, F.OwnerID, F.Name, F.Created, F.Size, F.LastModified, T.Name AS AccessType
                                      FROM Shares AS S
                                      JOIN Files AS F ON F.ID = S.FileID
                                      JOIN AccessTypes AS T ON T.ID = S.AccessTypeID
                                      WHERE S.PersonApprovedID = @PersonID
                                      AND NOT EXISTS 
									  (SELECT S2.ID FROM Shares AS S2 WHERE S2.PersonApprovedID = @PersonID AND S2.ID IN (SELECT ParentID FROM GetFileParents(F.ID)));

END
GO
/****** Object:  StoredProcedure [dbo].[sp_RemoveDir]    Script Date: 12/2/2019 1:51:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_RemoveDir] 
	@DirID UNIQUEIDENTIFIER
AS
BEGIN
	

DECLARE @Entites TABLE (ID UNIQUEIDENTIFIER, StorageType VARCHAR(1), Level INT);

			WITH dirsReqursive AS 
            ( 
				SELECT ID, ParentID, 'D' AS StorageType, 1 AS Level 
				FROM Directories 
				WHERE ID = @DirID
				UNION ALL 
				SELECT dirs.ID, dirs.ParentID, 'D' AS StorageType, dirsReqursive.Level+1 AS Level 
				FROM Directories AS dirs 
				INNER JOIN dirsReqursive 
				ON dirs.ParentID = dirsReqursive.ID 
				WHERE dirs.ID != (CAST(0x0 AS UNIQUEIDENTIFIER))
				UNION ALL 
				SELECT files.ID, files.ParentID, 'F' AS StorageType, dirsReqursive.Level+1 AS Level 
				FROM Files AS files 
				INNER JOIN dirsReqursive 
				ON files.ParentID = dirsReqursive.ID 
            ) 
            INSERT INTO @Entites (ID, StorageType, Level) (SELECT ID, StorageType, Level FROM dirsReqursive)

DECLARE @Count INT = (SELECT COUNT(*) FROM @Entites); 

DELETE FROM Files WHERE ID IN (SELECT ID FROM @Entites WHERE StorageType = 'F');
DELETE FROM Directories WHERE ID IN (SELECT ID FROM @Entites WHERE StorageType = 'D' ORDER BY Level ASC OFFSET 0 ROWS)

SELECT @Count



END
GO

-- <- Generated
-------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------------------------

INSERT INTO AccessTypes VALUES (0, 'NONE'), (1, 'READ'), (2, 'WRITE');
INSERT INTO Users (ID, Name, PasswordHash, Salt) VALUES ( (CAST(0x0 AS UNIQUEIDENTIFIER)), '', '', '');
INSERT INTO Directories (ID, ParentID, OwnerID, Name) VALUES ((CAST(0x0 AS UNIQUEIDENTIFIER)), NULL, (CAST(0x0 AS UNIQUEIDENTIFIER)), '');
GO