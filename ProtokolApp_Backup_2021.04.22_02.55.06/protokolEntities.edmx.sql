
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 01/07/2021 11:30:32
-- Generated from EDMX file: C:\Users\midhat.ademovic\source\repos\Protokol2\protokol\ProtokolApp\protokolEntities.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [protokol];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_dokument_protokol]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[dokument] DROP CONSTRAINT [FK_dokument_protokol];
GO
IF OBJECT_ID(N'[dbo].[FK_korisnik_sluzbe]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[korisnik] DROP CONSTRAINT [FK_korisnik_sluzbe];
GO
IF OBJECT_ID(N'[dbo].[FK_protokol_sluzbe]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[protokol] DROP CONSTRAINT [FK_protokol_sluzbe];
GO
IF OBJECT_ID(N'[dbo].[FK_protokol_tip]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[protokol] DROP CONSTRAINT [FK_protokol_tip];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[dokument]', 'U') IS NOT NULL
    DROP TABLE [dbo].[dokument];
GO
IF OBJECT_ID(N'[dbo].[korisnik]', 'U') IS NOT NULL
    DROP TABLE [dbo].[korisnik];
GO
IF OBJECT_ID(N'[dbo].[protokol]', 'U') IS NOT NULL
    DROP TABLE [dbo].[protokol];
GO
IF OBJECT_ID(N'[dbo].[sluzbe]', 'U') IS NOT NULL
    DROP TABLE [dbo].[sluzbe];
GO
IF OBJECT_ID(N'[dbo].[tip]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tip];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'dokument'
CREATE TABLE [dbo].[dokument] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [ID_protokola] int  NOT NULL,
    [TipDokumenta] int  NOT NULL,
    [Dokument] varbinary(max)  NULL,
    [Filename] nvarchar(255)  NULL,
    [Izbrisan] smallint  NOT NULL,
    [Opis] nvarchar(255)  NULL,
    [Racunar] nvarchar(255)  NULL,
    [DatumUnosa] datetime  NULL,
    [DatumUnosa1] datetime  NULL
);
GO

-- Creating table 'korisnik'
CREATE TABLE [dbo].[korisnik] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [ID_sluzbe] int  NOT NULL,
    [Naziv] nvarchar(50)  NULL,
    [canedit] int  NULL,
    [cannew] int  NULL,
    [caninsertfile] int  NULL,
    [canexportexcel] int  NULL,
    [canexportexcel1] int  NULL
);
GO

-- Creating table 'protokol'
CREATE TABLE [dbo].[protokol] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [ID_sluzbe] int  NOT NULL,
    [redni_broj] int  NULL,
    [datum] datetime  NULL,
    [datum_distribucije] datetime  NULL,
    [predmet] nvarchar(250)  NULL,
    [veza] nvarchar(250)  NULL,
    [razvod] nvarchar(250)  NULL,
    [oznaka_registratora] nvarchar(50)  NULL,
    [oznaka_dopisa] nvarchar(50)  NULL,
    [dostava_dopisa] nvarchar(50)  NULL,
    [napomena] nvarchar(250)  NULL,
    [arhiva] nvarchar(50)  NULL,
    [izbrisan] int  NOT NULL,
    [ID_tipa] int  NOT NULL,
    [Racunar] nvarchar(255)  NULL,
    [tipProtokola_SP] nvarchar(255)  NULL,
    [brojUlaznogProtokola_SP] nvarchar(255)  NULL,
    [datumPrijema_SP] datetime  NULL,
    [signirano_SP] nvarchar(255)  NULL,
    [vrstaDokumenta_sp] nvarchar(255)  NULL,
    [sifraDokumenta_SP] nvarchar(255)  NULL,
    [nacinPrijema_SP] nvarchar(255)  NULL,
    [lokacijaCZK_SP] nvarchar(255)  NULL,
    [sifraProstora_SP] nvarchar(255)  NULL,
    [nazivKorisnikaUsluge_SP] nvarchar(255)  NULL,
    [adresaProstora_SP] nvarchar(255)  NULL,
    [rokZaOdgovor_SP] datetime  NULL,
    [brojRjesenja_SP] nvarchar(255)  NULL,
    [vrstaRjesenja_SP] nvarchar(255)  NULL,
    [pripremioRjesenje_SP] nvarchar(255)  NULL,
    [iznosPoRjesenju_SP] decimal(18,2)  NULL,
    [brojIzlaznogProtokola_SP] nvarchar(255)  NULL,
    [datumOdgovora_SP] datetime  NULL,
    [statusReklamacije_SP] nvarchar(255)  NULL,
    [podbroj_Kabinet] nvarchar(255)  NULL,
    [posiljalac_Kabinet] nvarchar(255)  NULL,
    [redniBroj_Kabinet] nvarchar(50)  NULL,
    [CntDokumenata] int  NULL,
    [tipProtokola_SP1] nvarchar(255)  NULL,
    [brojUlaznogProtokola_SP1] nvarchar(255)  NULL,
    [datumPrijema_SP1] datetime  NULL,
    [signirano_SP1] nvarchar(255)  NULL,
    [vrstaDokumenta_sp1] nvarchar(255)  NULL,
    [sifraDokumenta_SP1] nvarchar(255)  NULL,
    [nacinPrijema_SP1] nvarchar(255)  NULL,
    [lokacijaCZK_SP1] nvarchar(255)  NULL,
    [sifraProstora_SP1] nvarchar(255)  NULL,
    [nazivKorisnikaUsluge_SP1] nvarchar(255)  NULL,
    [adresaProstora_SP1] nvarchar(255)  NULL,
    [rokZaOdgovor_SP1] datetime  NULL,
    [brojRjesenja_SP1] nvarchar(255)  NULL,
    [vrstaRjesenja_SP1] nvarchar(255)  NULL,
    [pripremioRjesenje_SP1] nvarchar(255)  NULL,
    [iznosPoRjesenju_SP1] decimal(18,2)  NULL,
    [brojIzlaznogProtokola_SP1] nvarchar(255)  NULL,
    [datumOdgovora_SP1] datetime  NULL,
    [statusReklamacije_SP1] nvarchar(255)  NULL,
    [podbroj_Kabinet1] nvarchar(255)  NULL,
    [posiljalac_Kabinet1] nvarchar(255)  NULL,
    [redniBroj_Kabinet1] nvarchar(50)  NULL
);
GO

-- Creating table 'sluzbe'
CREATE TABLE [dbo].[sluzbe] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Naziv] nvarchar(50)  NULL,
    [Naziv_forme] nvarchar(50)  NULL,
    [Naziv_forme1] nvarchar(50)  NULL
);
GO

-- Creating table 'tip'
CREATE TABLE [dbo].[tip] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Naziv] nvarchar(50)  NULL
);
GO

-- Creating table 'korisniksluzbe'
CREATE TABLE [dbo].[korisniksluzbe] (
    [korisnik1_ID] int  NOT NULL,
    [sluzbe1_ID] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [ID] in table 'dokument'
ALTER TABLE [dbo].[dokument]
ADD CONSTRAINT [PK_dokument]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'korisnik'
ALTER TABLE [dbo].[korisnik]
ADD CONSTRAINT [PK_korisnik]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'protokol'
ALTER TABLE [dbo].[protokol]
ADD CONSTRAINT [PK_protokol]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'sluzbe'
ALTER TABLE [dbo].[sluzbe]
ADD CONSTRAINT [PK_sluzbe]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'tip'
ALTER TABLE [dbo].[tip]
ADD CONSTRAINT [PK_tip]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [korisnik1_ID], [sluzbe1_ID] in table 'korisniksluzbe'
ALTER TABLE [dbo].[korisniksluzbe]
ADD CONSTRAINT [PK_korisniksluzbe]
    PRIMARY KEY CLUSTERED ([korisnik1_ID], [sluzbe1_ID] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [ID_protokola] in table 'dokument'
ALTER TABLE [dbo].[dokument]
ADD CONSTRAINT [FK_dokument_protokol]
    FOREIGN KEY ([ID_protokola])
    REFERENCES [dbo].[protokol]
        ([ID])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_dokument_protokol'
CREATE INDEX [IX_FK_dokument_protokol]
ON [dbo].[dokument]
    ([ID_protokola]);
GO

-- Creating foreign key on [ID_sluzbe] in table 'protokol'
ALTER TABLE [dbo].[protokol]
ADD CONSTRAINT [FK_protokol_sluzbe]
    FOREIGN KEY ([ID_sluzbe])
    REFERENCES [dbo].[sluzbe]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_protokol_sluzbe'
CREATE INDEX [IX_FK_protokol_sluzbe]
ON [dbo].[protokol]
    ([ID_sluzbe]);
GO

-- Creating foreign key on [ID_tipa] in table 'protokol'
ALTER TABLE [dbo].[protokol]
ADD CONSTRAINT [FK_protokol_tip]
    FOREIGN KEY ([ID_tipa])
    REFERENCES [dbo].[tip]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_protokol_tip'
CREATE INDEX [IX_FK_protokol_tip]
ON [dbo].[protokol]
    ([ID_tipa]);
GO

-- Creating foreign key on [korisnik1_ID] in table 'korisniksluzbe'
ALTER TABLE [dbo].[korisniksluzbe]
ADD CONSTRAINT [FK_korisniksluzbe_korisnik]
    FOREIGN KEY ([korisnik1_ID])
    REFERENCES [dbo].[korisnik]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [sluzbe1_ID] in table 'korisniksluzbe'
ALTER TABLE [dbo].[korisniksluzbe]
ADD CONSTRAINT [FK_korisniksluzbe_sluzbe]
    FOREIGN KEY ([sluzbe1_ID])
    REFERENCES [dbo].[sluzbe]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_korisniksluzbe_sluzbe'
CREATE INDEX [IX_FK_korisniksluzbe_sluzbe]
ON [dbo].[korisniksluzbe]
    ([sluzbe1_ID]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------