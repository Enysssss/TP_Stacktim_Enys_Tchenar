USE Stacktim;
GO

CREATE TABLE Competitors (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nickname NVARCHAR(50) NOT NULL UNIQUE,
    EmailAddress NVARCHAR(100) NOT NULL UNIQUE,
    RankLevel NVARCHAR(20) NOT NULL CHECK (RankLevel IN ('Bronze', 'Silver', 'Gold', 'Platinum', 'Diamond', 'Master')),
    AccumulatedPoints INT NOT NULL DEFAULT 0 CHECK (AccumulatedPoints >= 0),
    EnrollmentDate DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE UNIQUE INDEX IX_Competitors_Nickname ON Competitors(Nickname);
CREATE UNIQUE INDEX IX_Competitors_Email ON Competitors(EmailAddress);

CREATE TABLE Squads (
    Id INT PRIMARY KEY IDENTITY(1,1),
    SquadName NVARCHAR(100) NOT NULL UNIQUE,
    Abbreviation NCHAR(3) NOT NULL UNIQUE CHECK (Abbreviation = UPPER(Abbreviation) COLLATE Latin1_General_CS_AS),
    LeaderId INT NOT NULL,
    FoundationDate DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Squads_Leader FOREIGN KEY (LeaderId) REFERENCES Competitors(Id)
);

CREATE UNIQUE INDEX IX_Squads_Tag ON Squads(Abbreviation);

CREATE TABLE SquadMembers (
    SquadId INT NOT NULL,
    CompetitorId INT NOT NULL,
    Position INT NOT NULL CHECK (Position IN (0, 1, 2)), 
    MembershipDate DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_SquadMembers PRIMARY KEY (SquadId, CompetitorId),
    CONSTRAINT FK_SquadMembers_Squad FOREIGN KEY (SquadId) REFERENCES Squads(Id) ON DELETE CASCADE,
    CONSTRAINT FK_SquadMembers_Competitor FOREIGN KEY (CompetitorId) REFERENCES Competitors(Id) ON DELETE CASCADE
);
GO