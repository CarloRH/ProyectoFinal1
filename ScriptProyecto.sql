CREATE DATABASE InvestigacionesAI;
GO

USE InvestigacionesAI;
GO

CREATE TABLE Investigaciones (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Consulta NVARCHAR(MAX),
    Resultado NVARCHAR(MAX),
    Fecha DATETIME
);