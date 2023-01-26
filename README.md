# Comakership

De backend heeft wat voorwerk nodig. Hireonder is het beschreven:

Comakership Backend preparations
*Visual Studio
*Microsoft SQL Server Studio
- Server name: (localdb)\mssqllocaldb

MockData Query's anders loop je tegen errors!!!

// Mockdata zodat een student een opleiding kan kiezen om te registreren
Programs aanmaken
insert into [Comakerships].[dbo].[Programs] (Name) values ('Business IT & Management')
insert into [Comakerships].[dbo].[Programs] (Name) values ('Elektrotechniek')
insert into [Comakerships].[dbo].[Programs] (Name) values ('Applied Data Science & Artificial Intelligence')

// Mockdata zodat een company een comakership kan aanmaken
PurchaseKeys aanmaken
USE [Comakerships]
GO
INSERT INTO [dbo].[PurchaseKeys]
           ([Key]
           ,[Claimed])
     VALUES
           (012345, 0)
GO

// Comakership statussen aanmaken
insert into dbo.ComakershipStatuses (Name) values ('Not started')
insert into dbo.ComakershipStatuses (Name) values ('Started')
insert into dbo.ComakershipStatuses (Name) values ('Finished')

