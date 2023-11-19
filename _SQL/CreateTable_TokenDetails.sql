DROP TABLE IF EXISTS beer_exchange.TokenDetails;

CREATE TABLE beer_exchange.TokenDetails (
    UserId              INT             NOT NULL,
    TokenId             NVARCHAR(36)    NOT NULL,
    TokenExpiryUtc      DATETIME        NOT NULL,
    RefreshToken        NVARCHAR(36)    NOT NULL,
    RefreshExpiryUtc    DATETIME        NOT NULL,
    PRIMARY KEY (UserId),
    CONSTRAINT UC_TokenDetails_TokenId UNIQUE (TokenId)
);