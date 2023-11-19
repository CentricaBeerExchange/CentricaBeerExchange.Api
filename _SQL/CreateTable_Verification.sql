DROP TABLE IF EXISTS beer_exchange.Verification;
CREATE TABLE beer_exchange.Verification (
    Email           NVARCHAR(255)   NOT NULL,
    CodeHash        NVARCHAR(100)   NOT NULL,
    CreatedAtUtc    DATETIME        NOT NULL,
    ValidUntilUtc   DATETIME        NOT NULL,
    PRIMARY KEY (Email)
);