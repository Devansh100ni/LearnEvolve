CREATE TABLE [dbo].[Users] (
    [UserId] INT IDENTITY(1,1) PRIMARY KEY,       -- Auto-incrementing ID
    [UserName] NVARCHAR(100) NOT NULL,            -- Username
    [Email] NVARCHAR(150) NOT NULL UNIQUE,        -- Unique email
    [PasswordHash] NVARCHAR(255) NOT NULL,        -- Store hashed password
    [FirstName] NVARCHAR(100) NULL,               -- Optional first name
    [LastName] NVARCHAR(100) NULL,                -- Optional last name
    [Role] NVARCHAR(50) NOT NULL DEFAULT 'User',  -- Role (User/Admin)
    [IsActive] BIT NOT NULL DEFAULT 1,            -- Active status
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(), -- Created timestamp
    [UpdatedAt] DATETIME NULL                     -- Updated timestamp
);
