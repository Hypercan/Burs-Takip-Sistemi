/*
  Burs Takip Sistemi (BURSTAR) - Veritabanı Şema Kurulum Script'i
  Kaynak: Migrations/20260512173712_InitialCreate
  DBMS: Microsoft SQL Server
  Not: Sunum öncesi test ortamında çalıştırın. Üretimde EF migration tercih edilir.
*/

-- Hedef veritabanını seçin (Azure SQL: BursDb)
-- USE [BursDb];
-- GO

/* ============================================================
   0. Mevcut tabloları temizle (sıfırdan kurulum - İSTEĞE BAĞLI)
   ============================================================ */
IF OBJECT_ID(N'dbo.ApplicationDocuments', N'U') IS NOT NULL DROP TABLE dbo.ApplicationDocuments;
IF OBJECT_ID(N'dbo.Applications', N'U') IS NOT NULL DROP TABLE dbo.Applications;
IF OBJECT_ID(N'dbo.Documents', N'U') IS NOT NULL DROP TABLE dbo.Documents;
IF OBJECT_ID(N'dbo.ScholarshipPrograms', N'U') IS NOT NULL DROP TABLE dbo.ScholarshipPrograms;
IF OBJECT_ID(N'dbo.SystemLogs', N'U') IS NOT NULL DROP TABLE dbo.SystemLogs;
IF OBJECT_ID(N'dbo.StudentProfiles', N'U') IS NOT NULL DROP TABLE dbo.StudentProfiles;
IF OBJECT_ID(N'dbo.InstitutionProfiles', N'U') IS NOT NULL DROP TABLE dbo.InstitutionProfiles;
IF OBJECT_ID(N'dbo.Users', N'U') IS NOT NULL DROP TABLE dbo.Users;
GO

/* ============================================================
   1. Users - Merkezi kimlik tablosu
   ============================================================ */
CREATE TABLE dbo.Users (
    UserID          INT             NOT NULL IDENTITY(1, 1),
    Email           NVARCHAR(MAX)   NOT NULL,
    PasswordHash    NVARCHAR(MAX)   NOT NULL,
    Role            NVARCHAR(MAX)   NOT NULL,  -- student / institution / admin
    ApprovalStatus  NVARCHAR(MAX)   NOT NULL,  -- beklemede / onaylandi / reddedildi
    CreatedAt       DATETIME2       NOT NULL,
    CONSTRAINT PK_Users PRIMARY KEY (UserID)
);
GO

/* ============================================================
   2. InstitutionProfiles - Kurum profilleri
   ============================================================ */
CREATE TABLE dbo.InstitutionProfiles (
    InstitutionID           INT             NOT NULL IDENTITY(1, 1),
    UserID                  INT             NOT NULL,
    InstitutionName         NVARCHAR(MAX)   NOT NULL,
    EntityType              NVARCHAR(MAX)   NOT NULL,  -- kurum / sahis
    IdentityNumber          NVARCHAR(MAX)   NOT NULL,
    TaxCertificatePath      NVARCHAR(MAX)   NOT NULL,
    AuthorizedPersonName    NVARCHAR(MAX)   NOT NULL,
    AuthorizedPersonPhone   NVARCHAR(MAX)   NOT NULL,
    AuthorizedPersonEmail   NVARCHAR(MAX)   NOT NULL,
    CONSTRAINT PK_InstitutionProfiles PRIMARY KEY (InstitutionID),
    CONSTRAINT FK_InstitutionProfiles_Users_UserID
        FOREIGN KEY (UserID) REFERENCES dbo.Users (UserID)
        ON DELETE NO ACTION
);
GO

CREATE INDEX IX_InstitutionProfiles_UserID ON dbo.InstitutionProfiles (UserID);
GO

/* ============================================================
   3. StudentProfiles - Öğrenci profilleri
   ============================================================ */
CREATE TABLE dbo.StudentProfiles (
    StudentID           INT             NOT NULL IDENTITY(1, 1),
    UserID              INT             NOT NULL,
    FirstName           NVARCHAR(MAX)   NOT NULL,
    LastName            NVARCHAR(MAX)   NOT NULL,
    BirthDate           DATETIME2       NOT NULL,
    Gender              NVARCHAR(MAX)   NOT NULL,
    DisabilityStatus    BIT             NOT NULL CONSTRAINT DF_StudentProfiles_DisabilityStatus DEFAULT (0),
    Department          NVARCHAR(MAX)   NOT NULL,
    School              NVARCHAR(MAX)   NOT NULL,
    Phone               NVARCHAR(MAX)   NOT NULL,
    Address             NVARCHAR(MAX)   NOT NULL,
    IBAN                NVARCHAR(MAX)   NOT NULL,
    BankName            NVARCHAR(MAX)   NOT NULL,
    PhotoPath           NVARCHAR(MAX)   NOT NULL,
    CONSTRAINT PK_StudentProfiles PRIMARY KEY (StudentID),
    CONSTRAINT FK_StudentProfiles_Users_UserID
        FOREIGN KEY (UserID) REFERENCES dbo.Users (UserID)
        ON DELETE NO ACTION
);
GO

CREATE INDEX IX_StudentProfiles_UserID ON dbo.StudentProfiles (UserID);
GO

/* ============================================================
   4. SystemLogs - Sistem denetim kayıtları
   ============================================================ */
CREATE TABLE dbo.SystemLogs (
    LogID       INT             NOT NULL IDENTITY(1, 1),
    UserID      INT             NULL,
    Action      NVARCHAR(MAX)   NOT NULL,
    IPAddress   NVARCHAR(MAX)   NOT NULL,
    Timestamp   DATETIME2       NOT NULL,
    Details     NVARCHAR(MAX)   NOT NULL,
    CONSTRAINT PK_SystemLogs PRIMARY KEY (LogID),
    CONSTRAINT FK_SystemLogs_Users_UserID
        FOREIGN KEY (UserID) REFERENCES dbo.Users (UserID)
        ON DELETE NO ACTION
);
GO

CREATE INDEX IX_SystemLogs_UserID ON dbo.SystemLogs (UserID);
GO

/* ============================================================
   5. ScholarshipPrograms - Burs ilanları
   ============================================================ */
CREATE TABLE dbo.ScholarshipPrograms (
    ProgramID               INT             NOT NULL IDENTITY(1, 1),
    InstitutionID           INT             NOT NULL,
    ProgramName             NVARCHAR(MAX)   NOT NULL,
    Amount                  DECIMAL(18, 2)  NOT NULL,
    DurationMonths          INT             NULL,
    Quota                   INT             NULL,
    GenderCriteria          NVARCHAR(MAX)   NOT NULL,
    DepartmentCriteria      NVARCHAR(MAX)   NOT NULL,
    MinGPA                  DECIMAL(4, 2)   NULL,
    Status                  NVARCHAR(MAX)   NOT NULL,
    ApplicationDeadline     DATETIME2       NOT NULL,
    SubmissionDeadline      DATETIME2       NOT NULL,
    AdminNote               NVARCHAR(MAX)   NOT NULL,
    CreatedAt               DATETIME2       NOT NULL,
    SubmittedAt             DATETIME2       NULL,
    ApprovedAt              DATETIME2       NULL,
    CONSTRAINT PK_ScholarshipPrograms PRIMARY KEY (ProgramID),
    CONSTRAINT FK_ScholarshipPrograms_InstitutionProfiles_InstitutionID
        FOREIGN KEY (InstitutionID) REFERENCES dbo.InstitutionProfiles (InstitutionID)
        ON DELETE NO ACTION
);
GO

CREATE INDEX IX_ScholarshipPrograms_InstitutionID ON dbo.ScholarshipPrograms (InstitutionID);
GO

/* ============================================================
   6. Documents - Öğrenci belgeleri
   ============================================================ */
CREATE TABLE dbo.Documents (
    DocumentID      INT             NOT NULL IDENTITY(1, 1),
    StudentID       INT             NOT NULL,
    DocumentType    NVARCHAR(MAX)   NOT NULL,
    FilePath        NVARCHAR(MAX)   NOT NULL,
    UploadedAt      DATETIME2       NOT NULL,
    CONSTRAINT PK_Documents PRIMARY KEY (DocumentID),
    CONSTRAINT FK_Documents_StudentProfiles_StudentID
        FOREIGN KEY (StudentID) REFERENCES dbo.StudentProfiles (StudentID)
        ON DELETE NO ACTION
);
GO

CREATE INDEX IX_Documents_StudentID ON dbo.Documents (StudentID);
GO

/* ============================================================
   7. Applications - Burs başvuruları
   ============================================================ */
CREATE TABLE dbo.Applications (
    ApplicationID       INT             NOT NULL IDENTITY(1, 1),
    StudentID           INT             NOT NULL,
    ProgramID           INT             NOT NULL,
    Status              NVARCHAR(MAX)   NOT NULL,
    AppliedAt           DATETIME2       NOT NULL,
    UpdatedAt           DATETIME2       NULL,
    InstitutionNote     NVARCHAR(MAX)   NOT NULL,
    CONSTRAINT PK_Applications PRIMARY KEY (ApplicationID),
    CONSTRAINT FK_Applications_StudentProfiles_StudentID
        FOREIGN KEY (StudentID) REFERENCES dbo.StudentProfiles (StudentID)
        ON DELETE NO ACTION,
    CONSTRAINT FK_Applications_ScholarshipPrograms_ProgramID
        FOREIGN KEY (ProgramID) REFERENCES dbo.ScholarshipPrograms (ProgramID)
        ON DELETE NO ACTION
);
GO

CREATE INDEX IX_Applications_StudentID ON dbo.Applications (StudentID);
CREATE INDEX IX_Applications_ProgramID ON dbo.Applications (ProgramID);
GO

/* ============================================================
   8. ApplicationDocuments - Başvuru-belge köprü tablosu
   ============================================================ */
CREATE TABLE dbo.ApplicationDocuments (
    AppDocID        INT             NOT NULL IDENTITY(1, 1),
    ApplicationID   INT             NOT NULL,
    DocumentID      INT             NOT NULL,
    Status          NVARCHAR(MAX)   NOT NULL,
    ReviewedAt      DATETIME2       NULL,
    ReviewedByID    INT             NULL,
    CONSTRAINT PK_ApplicationDocuments PRIMARY KEY (AppDocID),
    CONSTRAINT FK_ApplicationDocuments_Applications_ApplicationID
        FOREIGN KEY (ApplicationID) REFERENCES dbo.Applications (ApplicationID)
        ON DELETE NO ACTION,
    CONSTRAINT FK_ApplicationDocuments_Documents_DocumentID
        FOREIGN KEY (DocumentID) REFERENCES dbo.Documents (DocumentID)
        ON DELETE NO ACTION,
    CONSTRAINT FK_ApplicationDocuments_Users_ReviewedByID
        FOREIGN KEY (ReviewedByID) REFERENCES dbo.Users (UserID)
        ON DELETE NO ACTION
);
GO

CREATE INDEX IX_ApplicationDocuments_ApplicationID ON dbo.ApplicationDocuments (ApplicationID);
CREATE INDEX IX_ApplicationDocuments_DocumentID ON dbo.ApplicationDocuments (DocumentID);
CREATE INDEX IX_ApplicationDocuments_ReviewedByID ON dbo.ApplicationDocuments (ReviewedByID);
GO

PRINT N'BURSTAR veritabani semasi basariyla olusturuldu.';
GO
