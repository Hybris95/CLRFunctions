sp_configure 'clr enabled', 1
RECONFIGURE WITH OVERRIDE
-- Drop des fonctions pré-existantes
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.RegExMatches')) DROP FUNCTION dbo.RegExMatches
GO
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.RegExNbMatches')) DROP FUNCTION dbo.RegExNbMatches
GO
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.RegExMatchesSplit')) DROP FUNCTION dbo.RegExMatchesSplit
GO
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.RegExIsMatch')) DROP FUNCTION dbo.RegExIsMatch
GO
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.RegExMatch')) DROP FUNCTION dbo.RegExMatch
GO
-- Drop de l'assembly pré-existante puis recréation de celle-ci
IF EXISTS ( SELECT 1 FROM sys.assemblies asms WHERE asms.name = N'RegExFunction' ) DROP ASSEMBLY [RegExFunction]
CREATE ASSEMBLY RegExFunction FROM 'C:\CLR\Regex\RegularExpressionFunctions.dll' WITH PERMISSION_SET = SAFE
GO
-- Création des fonctions
CREATE FUNCTION dbo.RegExMatches(@pattern NVARCHAR(MAX), @sentence NVARCHAR(MAX), @separator NVARCHAR(MAX)) RETURNS NVARCHAR(MAX)
AS EXTERNAL NAME RegExFunction.[Deltadore.DotNet.Cbuisson.RegularExpressionFunctions].RegExMatches
GO
CREATE FUNCTION dbo.RegExNbMatches(@pattern NVARCHAR(MAX), @sentence NVARCHAR(MAX)) RETURNS INT
AS EXTERNAL NAME RegExFunction.[Deltadore.DotNet.Cbuisson.RegularExpressionFunctions].RegExNbMatches
GO
--CREATE FUNCTION dbo.RegExMatchesSplit(@pattern NVARCHAR(MAX), @sentence NVARCHAR(MAX)) RETURNS TABLE (Sujet NVARCHAR(MAX), Match NVARCHAR(MAX))
CREATE FUNCTION dbo.RegExMatchesSplit(@pattern NVARCHAR(MAX), @sentence NVARCHAR(MAX)) RETURNS TABLE (Match NVARCHAR(MAX))
AS EXTERNAL NAME RegExFunction.[Deltadore.DotNet.Cbuisson.RegularExpressionFunctions].RegExMatchesSplit
GO
CREATE FUNCTION dbo.RegExIsMatch(@pattern NVARCHAR(MAX), @sentence NVARCHAR(MAX)) RETURNS BIT
AS EXTERNAL NAME RegExFunction.[Deltadore.DotNet.Cbuisson.RegularExpressionFunctions].RegExIsMatch
GO
CREATE FUNCTION dbo.RegExMatch(@pattern NVARCHAR(MAX), @sentence NVARCHAR(MAX)) RETURNS NVARCHAR(MAX)
AS EXTERNAL NAME RegExFunction.[Deltadore.DotNet.Cbuisson.RegularExpressionFunctions].RegExMatch
GO
-- Test des fonctions
DECLARE @sentence NVARCHAR(MAX)
DECLARE @regex NVARCHAR(MAX)
DECLARE @regex2 NVARCHAR(MAX)
DECLARE @separator NVARCHAR(MAX)
SET @sentence = 'ABABCCADSQDJIOAZF JAIPZDJKL MNJKCXNjnaze iodjazpdjadpazdoa zdjio'
SET @regex = '[A-z]{6}\ '
SET @regex2 = '[A-z]{6}'
SET @separator = ';'
SELECT @regex as 'Regex', @sentence as 'Sentence', dbo.RegExMatch(@regex2, match) FROM dbo.RegExMatchesSplit(@regex, @sentence);
SELECT @regex as 'Regex', @sentence as 'Sentence', dbo.RegExMatches(@regex2, dbo.RegExMatches(@regex, @sentence, @separator), @separator)
SELECT @regex as 'Regex', @sentence as 'Sentence', dbo.RegExNbMatches(@regex,@sentence)
SELECT @regex as 'Regex', @sentence as 'Sentence', dbo.RegExIsMatch(@regex,@sentence)
SELECT @regex as 'Regex', @sentence as 'Sentence', dbo.RegExMatch(@regex2, dbo.RegExMatch(@regex,@sentence))
GO
-- Test des fonctions (spécifique Selligent)
SELECT nrid, titulaire, type, date_deb, dbo.RegExNbMatches('FG[0-9]{7}', sujet) as 'Nombre d''occurences', dbo.RegExMatches('FG[0-9]{7}', sujet, ';') as 'Sous-Chaînes concaténées', sujet FROM SYSADM.hi0
WHERE sujet IS NOT NULL
AND template IS NULL
AND dbo.RegExMatches('FG[0-9]{7}', sujet, ';') IS NOT NULL
ORDER BY 5 DESC, date_deb DESC, titulaire, nrid;

SELECT hi0.nrid, hi0.titulaire, hi0.type, hi0.date_deb, hi0.sujet, dbo.RegExNbMatches('FG[0-9]{7}', hi0.sujet), split.match FROM SYSADM.hi0 hi0
CROSS APPLY dbo.RegExMatchesSplit('FG[0-9]{7}', hi0.sujet) as split
WHERE hi0.sujet IS NOT NULL
AND hi0.template IS NULL
ORDER BY 6 DESC, date_deb DESC, titulaire, nrid;

SELECT hi0.nrid, hi0.titulaire, hi0.type, hi0.date_deb, hi0.sujet, dbo.RegExNbMatches('FG[0-9]{7}', hi0.sujet), split.match FROM SYSADM.hi0 hi0
OUTER APPLY dbo.RegExMatchesSplit('FG[0-9]{7}', hi0.sujet) as split
WHERE hi0.sujet IS NOT NULL
AND hi0.template IS NULL
ORDER BY 6 DESC, date_deb DESC, titulaire, nrid;