USE Stacktim;
GO

INSERT INTO Competitors (Nickname, EmailAddress, RankLevel, AccumulatedPoints) VALUES
('EnysTchenar', 'enys.tchenar@esport.com', 'Master', 9500),
('ShadowPhoenix', 'shadow.phoenix@gaming.net', 'Diamond', 7800),
('ThunderBolt', 'thunder.bolt@arena.io', 'Platinum', 6200),
('CrystalWave', 'crystal.wave@compete.org', 'Gold', 4500),
('BlazeFury', 'blaze.fury@tournament.com', 'Silver', 2800),
('NovaStrike', 'nova.strike@gaming.pro', 'Bronze', 1200);

INSERT INTO Squads (SquadName, Abbreviation, LeaderId) VALUES
('Eternal Legends', 'ETL', 1),
('Storm Warriors', 'STW', 2),
('Rising Phoenix', 'RPX', 3);

INSERT INTO SquadMembers (SquadId, CompetitorId, Position) VALUES
(1, 1, 0), 
(1, 4, 1), 
(1, 5, 2), 
(2, 2, 0), 
(2, 6, 1), 
(3, 3, 0); 
GO