DROP TABLE IF EXISTS beer_exchange.Users;
DROP TABLE IF EXISTS beer_exchange.UserRoles;

CREATE TABLE beer_exchange.UserRoles (
    Id      TINYINT         NOT NULL,
    Name    NVARCHAR(100)   NOT NULL,
    PRIMARY KEY (Id)
);

CREATE TABLE beer_exchange.Users (
    Id          INT             NOT NULL AUTO_INCREMENT,
    Email       NVARCHAR(255)   NOT NULL,
    Role        TINYINT         NOT NULL,
    Name        NVARCHAR(255)   NULL,
    Department  NVARCHAR(255)   NULL,
    PRIMARY KEY (Id),
    UNIQUE INDEX UX_Users_Email (Email ASC),
    FOREIGN KEY (Role)  REFERENCES beer_exchange.UserRoles(Id)
);

INSERT INTO beer_exchange.UserRoles (Id, Name)
VALUES (10, "User"), (20, "Editor"), (30, "Admin");

INSERT INTO beer_exchange.Users (Email, Role)
VALUES ("dev@local", 30);