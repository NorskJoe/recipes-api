-- Initial table creation if they do not exist (SQL Server T-SQL)
IF OBJECT_ID (N'dbo.Recipe', N'U') IS NULL BEGIN
CREATE TABLE dbo.Recipe (
  Id INT NOT NULL IDENTITY(1,1),
  Title VARCHAR(50) NOT NULL,
  Description VARCHAR(100),
  Servings INT,
  PrepTimeInMinutes INT,
  CookTimeInMinutes INT,
  Slug VARCHAR(50) NOT NULL,
  CreatedBy VARCHAR(50) NOT NULL,
  CreatedAt DATE NOT NULL,
  LastModifiedBy VARCHAR(50),
  LastModified DATE,
  PRIMARY KEY (Id)
);

END;

IF OBJECT_ID (N'dbo.Ingredient', N'U') IS NULL BEGIN
CREATE TABLE dbo.Ingredient (
  Id INT NOT NULL IDENTITY(1,1),
  Name VARCHAR(50) NOT NULL,
  PRIMARY KEY (Id)
);

END;


IF OBJECT_ID (N'dbo.IngredientMeasurement', N'U') IS NULL BEGIN
CREATE TABLE dbo.IngredientMeasurement (
  [Type] INT NOT NULL,
  DisplayName VARCHAR(50) NOT NULL,
  Abbreviation VARCHAR(10) NULL,
  CONSTRAINT PK_IngredientMeasurement PRIMARY KEY ([Type])
);

END;

-- Seed the enum values; skips any already present so re-runs are harmless.
INSERT INTO
  dbo.IngredientMeasurement ([Type], DisplayName, Abbreviation)
SELECT
  v.[Type],
  v.DisplayName,
  v.Abbreviation
FROM
  (
    VALUES
      (0, 'Piece', NULL),
      (1, 'Gram', 'g'),
      (2, 'Kilogram', 'kg'),
      (3, 'Millilitre', 'ml'),
      (4, 'Litre', 'l'),
      (5, 'Teaspoon', 'tsp'),
      (6, 'Tablespoon', 'tbsp'),
      (7, 'Cup', 'cup'),
      (8, 'Other', NULL)
  ) AS v ([Type], DisplayName, Abbreviation)
WHERE
  NOT EXISTS (
    SELECT
      1
    FROM
      dbo.IngredientMeasurement m
    WHERE
      m.[Type] = v.[Type]
  );

IF OBJECT_ID (N'dbo.RecipeIngredient', N'U') IS NULL BEGIN
  CREATE TABLE dbo.RecipeIngredient (
    Id INT NOT NULL IDENTITY(1,1),
    RecipeId INT NOT NULL,
    IngredientId INT NOT NULL,
    Measurement INT NOT NULL,
    Quantity DECIMAL(9,3) NOT NULL,
    PRIMARY KEY (Id),
    CONSTRAINT FK_RecipeIngredient_Recipe
      FOREIGN KEY (RecipeId) REFERENCES dbo.Recipe (Id)
      ON DELETE CASCADE,
    CONSTRAINT FK_RecipeIngredient_Ingredient
      FOREIGN KEY (IngredientId) REFERENCES dbo.Ingredient (Id),
    CONSTRAINT FK_RecipeIngredient_Measurement
      FOREIGN KEY (Measurement) REFERENCES dbo.IngredientMeasurement ([Type])
  );
END;

IF OBJECT_ID (N'dbo.RecipeInstruction', N'U') IS NULL BEGIN
  CREATE TABLE dbo.RecipeInstruction (
    Id INT NOT NULL IDENTITY(1,1),
    RecipeId INT NOT NULL,
    StepNumber INT NOT NULL,
    [Text] VARCHAR(255) NOT NULL,
    PRIMARY KEY (Id),
    CONSTRAINT FK_RecipeInstruction_Recipe
      FOREIGN KEY (RecipeId) REFERENCES dbo.Recipe (Id)
      ON DELETE CASCADE
    );
END;

IF OBJECT_ID (N'dbo.Tag', N'U') IS NULL BEGIN
  CREATE TABLE dbo.Tag (
    Id INT NOT NULL IDENTITY(1,1),
    Name VARCHAR(20) NOT NULL,
    PRIMARY KEY (Id)
  );
END;

IF OBJECT_ID (N'dbo.RecipeTag', N'U') IS NULL BEGIN
  CREATE TABLE dbo.RecipeTag (
    Id INT NOT NULL IDENTITY(1,1),
    RecipeId INT NOT NULL,
    TagId INT NOT NULL,
    PRIMARY KEY (Id),
    CONSTRAINT FK_RecipeTag_Recipe
      FOREIGN KEY (RecipeId) REFERENCES dbo.Recipe (Id)
      ON DELETE CASCADE,
    CONSTRAINT FK_RecipeTag_Tag
      FOREIGN KEY (TagId) REFERENCES dbo.Tag (Id),
    CONSTRAINT UQ_RecipeTag 
      UNIQUE (RecipeId, TagId)
  );
END;
