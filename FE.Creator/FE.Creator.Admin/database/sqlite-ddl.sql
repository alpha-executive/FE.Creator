CREATE TABLE [AspNetRoles] (
    [Id]   NVARCHAR (128) NOT NULL PRIMARY KEY,
    [Name] NVARCHAR (256) NOT NULL
);
CREATE TABLE [AspNetUserClaims] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL PRIMARY KEY,
    [UserId]     NVARCHAR (128) NOT NULL,
    [ClaimType]  NVARCHAR  NULL,
    [ClaimValue] NVARCHAR  NULL,
    FOREIGN KEY(UserId) REFERENCES AspNetUsers(id)
);
CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] NVARCHAR (128) NOT NULL PRIMARY KEY,
    [ProviderKey]   NVARCHAR (128) NOT NULL,
    [UserId]        NVARCHAR (128) NOT NULL,
    FOREIGN KEY(UserId) REFERENCES AspNetUsers(id)
);
CREATE TABLE [AspNetUserRoles] (
    [UserId] NVARCHAR (128) NOT NULL,
    [RoleId] NVARCHAR (128) NOT NULL,
    FOREIGN KEY(UserId) REFERENCES AspNetUsers(id),
    FOREIGN KEY(RoleId) REFERENCES AspNetRoles(id),
    PRIMARY KEY (UserId, RoleId)
);
CREATE TABLE [AspNetUsers] (
    [Id]                   NVARCHAR (128) NOT NULL PRIMARY KEY,
    [Email]                NVARCHAR (256) NULL,
    [EmailConfirmed]       BIT            NOT NULL,
    [PasswordHash]         NVARCHAR ,
    [SecurityStamp]        NVARCHAR,
    [PhoneNumber]          NVARCHAR,
    [PhoneNumberConfirmed] BIT            NOT NULL,
    [TwoFactorEnabled]     BIT            NOT NULL,
    [LockoutEndDateUtc]    DATETIME,
    [LockoutEnabled]       BIT            NOT NULL,
    [AccessFailedCount]    INT            NOT NULL,
    [UserName]             NVARCHAR NOT NULL
);
CREATE TABLE [FileGeneralObjectField] (
    [GeneralObjectFieldID] INTEGER   NOT NULL PRIMARY KEY AUTOINCREMENT,
    [FileName]             NVARCHAR NULL,
    [FileUrl]              NVARCHAR NULL,
    [FileFullPath]         NVARCHAR NULL,
    [FileExtension]        NVARCHAR  NULL,
    [FileSize]             INT            NOT NULL,
    [FileCRC]              NVARCHAR  NULL,
    [Created]              DATETIME       NOT NULL,
    [Updated]              DATETIME       NOT NULL,
    FOREIGN KEY ([GeneralObjectFieldID]) REFERENCES [GeneralObjectField] ([GeneralObjectFieldID])
);
CREATE TABLE [GeneralObject] (
    [GeneralObjectID]           INTEGER   NOT NULL PRIMARY KEY AUTOINCREMENT,
    [GeneralObjectName]         NVARCHAR  NULL,
    [Created]                   DATETIME       NOT NULL,
    [CreatedBy]                 NVARCHAR  NULL,
    [Updated]                   DATETIME       NOT NULL,
    [UpdatedBy]                 NVARCHAR  NULL,
    [ObjectOwner]               NVARCHAR  NULL,
    [GeneralObjectDefinitionID] INT            NOT NULL,
    [IsDeleted]                 BIT            NOT NULL,
    FOREIGN KEY ([GeneralObjectDefinitionID]) REFERENCES [GeneralObjectDefinition] ([GeneralObjectDefinitionID])
);
CREATE TABLE [GeneralObjectDefinition] (
    [GeneralObjectDefinitionID]      INTEGER   NOT NULL PRIMARY KEY AUTOINCREMENT,
    [GeneralObjectDefinitionName]    NVARCHAR  NULL,
    [GeneralObjectDefinitionKey]     NVARCHAR  NULL,
    [GeneralObjectDefinitionGroupID] INT            NOT NULL,
    [IsDeleted]                      BIT            NOT NULL,
    [Updated]                        DATETIME       NOT NULL,
    [Created]                        DATETIME       NOT NULL,
    [ObjectOwner]                    NVARCHAR NULL,
    [UpdatedBy]                      NVARCHAR  NULL,
    FOREIGN KEY ([GeneralObjectDefinitionGroupID]) REFERENCES GeneralObjectDefinitionGroup([GeneralObjectDefinitionGroupID])
);
CREATE TABLE [GeneralObjectDefinitionField] (
    [GeneralObjectDefinitionFieldID]   INTEGER   NOT NULL PRIMARY KEY AUTOINCREMENT,
    [GeneralObjectDefinitionFieldName] NVARCHAR  NULL,
    [GeneralObjectDefinitionFieldKey]  NVARCHAR NULL,
    [GeneralObjectDefinitionFiledType] INT            NOT NULL,
    [GeneralObjectDefinitionID]        INT            NOT NULL,
   FOREIGN KEY ([GeneralObjectDefinitionID]) REFERENCES [GeneralObjectDefinition] ([GeneralObjectDefinitionID])
);
CREATE TABLE [GeneralObjectDefinitionGroup] (
    [GeneralObjectDefinitionGroupID]   INTEGER   NOT NULL PRIMARY KEY AUTOINCREMENT,
    [GeneralObjectDefinitionGroupName] NVARCHAR  NULL,
    [GeneralObjectDefinitionGroupKey]  NVARCHAR  NULL,
    [IsDeleted]                        BIT            NOT NULL,
    [ParentGroupID]                    INT            NULL,
    FOREIGN KEY(ParentGroupID) REFERENCES GeneralObjectDefinitionGroup(GeneralObjectDefinitionGroupID)
);
CREATE TABLE [GeneralObjectDefinitionSelectItem] (
    [GeneralObjectDefinitionSelectItemID] INTEGER   NOT NULL PRIMARY KEY AUTOINCREMENT,
    [SelectDisplayName]                   NVARCHAR  NULL,
    [SelectItemKey]                       NVARCHAR  NULL,
    [GeneralObjectDefinitionFieldID]      INT            NOT NULL,
    FOREIGN KEY ([GeneralObjectDefinitionFieldID]) REFERENCES [SingleSelectionDefinitionField] ([GeneralObjectDefinitionFieldID])
);
CREATE TABLE [GeneralObjectField] (
    [GeneralObjectFieldID]           INTEGER   NOT NULL PRIMARY KEY AUTOINCREMENT,
    [GeneralObjectDefinitionFieldID] INT NOT NULL,
    [GeneralObjectID]                INT NOT NULL,
    FOREIGN KEY ([GeneralObjectID]) REFERENCES [GeneralObject] ([GeneralObjectID]),
    FOREIGN KEY ([GeneralObjectDefinitionFieldID]) REFERENCES [GeneralObjectDefinitionField] ([GeneralObjectDefinitionFieldID])
);
CREATE TABLE [GeneralObjectReferenceField] (
    [GeneralObjectFieldID]   INTEGER   NOT NULL PRIMARY KEY AUTOINCREMENT,
    [ReferedGeneralObjectID] INT NOT NULL,
 FOREIGN KEY ([GeneralObjectFieldID]) REFERENCES [GeneralObjectField] ([GeneralObjectFieldID])
);
CREATE TABLE [ObjRefObjectDefinitionField] (
    [GeneralObjectDefinitionFieldID] INTEGER   NOT NULL PRIMARY KEY AUTOINCREMENT,
    [ReferedObjectDefinitionID]      INT NOT NULL,
   FOREIGN KEY ([GeneralObjectDefinitionFieldID]) REFERENCES [GeneralObjectDefinitionField] ([GeneralObjectDefinitionFieldID])
);
CREATE TABLE [PrimeGeneralObjectField] (
    [GeneralObjectFieldID] INTEGER   NOT NULL PRIMARY KEY AUTOINCREMENT,
    [StringValue]          NVARCHAR  NULL,
    [IntegerValue]         INT             NULL,
    [LongValue]            BIGINT          NULL,
    [DateTimeValue]        DATETIME        NULL,
    [NumberValue]          FLOAT (53)      NULL,
    [BinaryValue]          VARBINARY  NULL,
    FOREIGN KEY ([GeneralObjectFieldID]) REFERENCES [GeneralObjectField] ([GeneralObjectFieldID])
);
CREATE TABLE [PrimeObjectDefintionField] (
    [GeneralObjectDefinitionFieldID] INTEGER   NOT NULL PRIMARY KEY AUTOINCREMENT,
    [PrimeDataType]                  INT NOT NULL,
FOREIGN KEY ([GeneralObjectDefinitionFieldID]) REFERENCES [GeneralObjectDefinitionField] ([GeneralObjectDefinitionFieldID])
);
CREATE TABLE [SingleSelectionDefinitionField] (
    [GeneralObjectDefinitionFieldID] INTEGER   NOT NULL PRIMARY KEY AUTOINCREMENT,
   FOREIGN KEY ([GeneralObjectDefinitionFieldID]) REFERENCES GeneralObjectDefinitionField([GeneralObjectDefinitionFieldID])
);
CREATE TABLE [SingleSelectionGeneralObjectField] (
    [GeneralObjectFieldID] INTEGER   NOT NULL PRIMARY KEY AUTOINCREMENT,
    [SelectedItemID]       INT NOT NULL,
   FOREIGN KEY ([GeneralObjectFieldID]) REFERENCES [GeneralObjectField] ([GeneralObjectFieldID])
);
