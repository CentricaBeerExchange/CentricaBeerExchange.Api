##===========================================================================
## Drop and Create database
##===========================================================================
DROP SCHEMA IF EXISTS beer_exchange;

CREATE SCHEMA beer_test 
	DEFAULT CHARACTER SET utf8 
    COLLATE utf8_danish_ci;

##===========================================================================
## Create User and Auth tables
##===========================================================================
CREATE TABLE beer_exchange.UserRoles (
    Id      TINYINT         NOT NULL,
    Name    NVARCHAR(100)   NOT NULL,
    PRIMARY KEY (Id)
);

INSERT INTO beer_exchange.UserRoles (Id, Name)
VALUES (10, "User"), (20, "Editor"), (30, "Admin");

CREATE TABLE beer_exchange.Users (
    Id          INT             NOT NULL AUTO_INCREMENT,
    Email       NVARCHAR(255)   NOT NULL,
    Role        TINYINT         NOT NULL,
    Name        NVARCHAR(255)   NULL,
    Thumbnail	MEDIUMTEXT		NULL,
    PRIMARY KEY (Id),
    UNIQUE INDEX UX_Users_Email (Email ASC),
    FOREIGN KEY (Role) REFERENCES beer_exchange.UserRoles(Id)
);

INSERT INTO beer_exchange.Users (Email, Role)
VALUES ("dev@local", 30);

CREATE TABLE beer_exchange.Verification (
    Email           NVARCHAR(255)   NOT NULL,
    CodeHash        NVARCHAR(100)   NOT NULL,
    CreatedAtUtc    DATETIME        NOT NULL,
    ValidUntilUtc   DATETIME        NOT NULL,
    PRIMARY KEY (Email)
);

CREATE TABLE beer_exchange.TokenDetails (
    UserId              INT             NOT NULL,
    TokenId             NVARCHAR(36)    NOT NULL,
    TokenExpiryUtc      DATETIME        NOT NULL,
    RefreshToken        NVARCHAR(36)    NOT NULL,
    RefreshExpiryUtc    DATETIME        NOT NULL,
    PRIMARY KEY (UserId),
    FOREIGN KEY (UserId) REFERENCES beer_exchange.Users(Id)
    CONSTRAINT UC_TokenDetails_TokenId UNIQUE (TokenId)
);

##===========================================================================
## Create Beer and Tastings tables
##===========================================================================
CREATE TABLE beer_exchange.Styles (
    Id          SMALLINT        NOT NULL,
    Name        NVARCHAR(255)   NOT NULL,
    IsActive    BOOL            NOT NULL,
    CONSTRAINT PK_Styles_Id
        PRIMARY KEY (Id)
);

CREATE TABLE beer_exchange.Breweries (
    Id          INT             NOT NULL AUTO_INCREMENT,
    Name        NVARCHAR(255)   NOT NULL,
    UntappdId   NVARCHAR(255)   NULL,
    Location    NVARCHAR(255)   NULL,
    Type        NVARCHAR(255)   NULL,
    Thumbnail	MEDIUMTEXT		NULL,
    CONSTRAINT PK_Breweries_Id
        PRIMARY KEY (Id),
    CONSTRAINT UX_Breweries_Name
        UNIQUE (Name),
    CONSTRAINT IX_Breweries_UntappdId
        INDEX (UntappdId)
);
CREATE INDEX IX_Breweries_UntappdId ON beer_exchange.Breweries (UntappdId);

CREATE TABLE beer_exchange.Beers (
    Id          INT             NOT NULL AUTO_INCREMENT,
    Name        NVARCHAR(255)   NOT NULL,
    Brewery     INT             NOT NULL,
    Style       SMALLINT        NOT NULL,
    Rating      DECIMAL(4,3)    NULL,
    ABV         DECIMAL(4,2)    NULL,
    UntappdId   INT             NULL,
    CONSTRAINT PK_Beers_Id
        PRIMARY KEY (Id),
    CONSTRAINT FK_Beers_Breweries_Id
        FOREIGN KEY (Brewery)
        REFERENCES beer_exchange.Breweries (Id),
    CONSTRAINT FK_Beers_Styles_Id
        FOREIGN KEY (Style)
        REFERENCES beer_exchange.Styles (Id)
);
CREATE INDEX IX_Beers_UntappdId ON beer_exchange.Beers (UntappdId);

CREATE TABLE beer_exchange.Tastings (
    Id          INT             NOT NULL AUTO_INCREMENT,
    Date        Date            NOT NULL,
    Theme       NVARCHAR(100)   NULL,
    CONSTRAINT PK_Tastings_Id
        PRIMARY KEY (Id),
    CONSTRAINT UX_Tastings_Date
        UNIQUE (Date)
);

CREATE TABLE beer_exchange.TastingParticipants (
    TastingId   INT             NOT NULL,
    UserId      INT             NOT NULL,
    BeerId      INT             NOT NULL,
    CONSTRAINT PK_TastingParticipants_TastingIdUserId
        PRIMARY KEY (TastingId, UserId),
    CONSTRAINT FK_TastingParticipants_Tastings_Id
        FOREIGN KEY (TastingId) 
        REFERENCES beer_exchange.Tastings (Id),
    CONSTRAINT FK_TastingParticipants_Users_Id
        FOREIGN KEY (UserId) 
        REFERENCES beer_exchange.Users (Id),
    CONSTRAINT FK_TastingParticipants_Beers_Id
        FOREIGN KEY (BeerId) 
        REFERENCES beer_exchange.Beers (Id)
);

CREATE TABLE beer_exchange.TastingVotes (
    TastingId   INT             NOT NULL,
    UserId      INT             NOT NULL,
    VotedUserId INT             NOT NULL,    
    CONSTRAINT PK_TastingVotes_TastingIdUserId
        PRIMARY KEY (TastingId, UserId),
    CONSTRAINT FK_TastingVotes_Tastings_TastingId
        FOREIGN KEY (TastingId) 
        REFERENCES beer_exchange.Tastings (Id),
    CONSTRAINT FK_TastingVotes_Users_UserId
        FOREIGN KEY (UserId) 
        REFERENCES beer_exchange.Users (Id),
    CONSTRAINT FK_TastingVotes_Users_VotedUserId
        FOREIGN KEY (VotedUserId) 
        REFERENCES beer_exchange.Users (Id)
);
