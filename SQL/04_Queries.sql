USE Stacktim;
GO

SELECT Nickname, AccumulatedPoints
FROM Competitors
ORDER BY AccumulatedPoints DESC;

SELECT s.SquadName, c.Nickname, 
    CASE sm.Position 
        WHEN 0 THEN 'Leader'
        WHEN 1 THEN 'Active'
        WHEN 2 THEN 'Reserve'
    END AS RoleDescription
FROM SquadMembers sm
JOIN Squads s ON sm.SquadId = s.Id
JOIN Competitors c ON sm.CompetitorId = c.Id
ORDER BY s.SquadName, sm.Position;

SELECT s.SquadName, 
    COUNT(sm.CompetitorId) AS TotalMembers,
    AVG(CAST(c.AccumulatedPoints AS FLOAT)) AS AverageScore
FROM Squads s
LEFT JOIN SquadMembers sm ON s.Id = sm.SquadId
LEFT JOIN Competitors c ON sm.CompetitorId = c.Id
GROUP BY s.SquadName;

SELECT c.Nickname, c.EmailAddress, c.RankLevel
FROM Competitors c
WHERE NOT EXISTS (
    SELECT 1 FROM SquadMembers sm WHERE sm.CompetitorId = c.Id
);

SELECT RankLevel, COUNT(*) AS PlayerCount
FROM Competitors
GROUP BY RankLevel
ORDER BY 
    CASE RankLevel
        WHEN 'Master' THEN 6
        WHEN 'Diamond' THEN 5
        WHEN 'Platinum' THEN 4
        WHEN 'Gold' THEN 3
        WHEN 'Silver' THEN 2
        WHEN 'Bronze' THEN 1
    END DESC;
GO