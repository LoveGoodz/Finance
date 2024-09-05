CREATE TABLE [Company] (
  [ID] int PRIMARY KEY IDENTITY(1, 1),
  [Name] varchar(100),
  [Adress] varchar(255),
  [PhoneNumber] varchar(20),
  [Email] varchar(100),
  [CreatedAt] datetime DEFAULT (CURRENT_TIMESTAMP),
  [UpdatedAt] datetime
)
GO

CREATE TABLE [Stock] (
  [ID] int PRIMARY KEY IDENTITY(1, 1),
  [CompanyID] int,
  [Name] varchar(100),
  [Quantity] int,
  [UnitPrice] decimal(10,2),
  [CreatedAt] datetime,
  [UpdatedAt] datetime
)
GO

CREATE TABLE [Customer] (
  [ID] int PRIMARY KEY IDENTITY(1, 1),
  [CompanyID] int,
  [Name] varchar(100),
  [Adress] varchar(255),
  [PhoneNumber] varchar(20),
  [Email] varchar(100),
  [CreatedAt] datetime,
  [UpdatedAt] datetime
)
GO

CREATE TABLE [Invoice] (
  [ID] int PRIMARY KEY IDENTITY(1, 1),
  [CompanyID] int,
  [CustomerID] int,
  [InvoiceDate] datetime,
  [Series] varchar(20),
  [Status] varchar(20),
  [TotalAmount] decimal(10,2),
  [CreatedAt] datetime,
  [UpdatedAt] datetime
)
GO

CREATE TABLE [InvoiceDetails] (
  [ID] int PRIMARY KEY IDENTITY(1, 1),
  [InvoiceID] int,
  [StockID] int,
  [Quantity] int,
  [UnitPrice] decimal(10,2),
  [TotalPrice] decimal(10,2),
  [CreatedAt] datetime,
  [UpdatedAt] datetime
)
GO

CREATE TABLE [StockTrans] (
  [ID] int PRIMARY KEY IDENTITY(1, 1),
  [StockID] int,
  [InvoiceDetailsID] int,
  [TransactionType] varchar(20),
  [Quantity] int,
  [CreatedAt] datetime
)
GO

CREATE TABLE [ActTrans] (
  [ID] int PRIMARY KEY IDENTITY(1, 1),
  [CustomerID] int,
  [InvoiceID] int,
  [TransactionType] varchar(20),
  [Amount] decimal(10,2),
  [CreatedAt] datetime
)
GO

CREATE TABLE [Balance] (
  [ID] int PRIMARY KEY IDENTITY(1, 1),
  [CompanyID] int,
  [StockID] int,
  [CustomerID] int,
  [TotalStock] decimal(10,2),
  [TotalDebit] decimal(10,2),
  [TotalCredit] decimal(10,2),
  [CreatedAt] datetime,
  [UpdatedAt] datetime
)
GO

ALTER TABLE [Stock] ADD FOREIGN KEY ([CompanyID]) REFERENCES [Company] ([ID])
GO

ALTER TABLE [Customer] ADD FOREIGN KEY ([CompanyID]) REFERENCES [Company] ([ID])
GO

ALTER TABLE [Invoice] ADD FOREIGN KEY ([CompanyID]) REFERENCES [Company] ([ID])
GO

ALTER TABLE [Invoice] ADD FOREIGN KEY ([CustomerID]) REFERENCES [Customer] ([ID])
GO

ALTER TABLE [InvoiceDetails] ADD FOREIGN KEY ([InvoiceID]) REFERENCES [Invoice] ([ID])
GO

ALTER TABLE [InvoiceDetails] ADD FOREIGN KEY ([StockID]) REFERENCES [Stock] ([ID])
GO

ALTER TABLE [StockTrans] ADD FOREIGN KEY ([StockID]) REFERENCES [Stock] ([ID])
GO

ALTER TABLE [StockTrans] ADD FOREIGN KEY ([InvoiceDetailsID]) REFERENCES [InvoiceDetails] ([ID])
GO

ALTER TABLE [ActTrans] ADD FOREIGN KEY ([CustomerID]) REFERENCES [Customer] ([ID])
GO

ALTER TABLE [ActTrans] ADD FOREIGN KEY ([InvoiceID]) REFERENCES [Invoice] ([ID])
GO

ALTER TABLE [Balance] ADD FOREIGN KEY ([CompanyID]) REFERENCES [Company] ([ID])
GO

ALTER TABLE [Balance] ADD FOREIGN KEY ([StockID]) REFERENCES [Stock] ([ID])
GO

ALTER TABLE [Balance] ADD FOREIGN KEY ([CustomerID]) REFERENCES [Customer] ([ID])
GO
