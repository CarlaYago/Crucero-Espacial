SELECT Anagrama, COUNT(*) Num
FROM LexicoEsp
WHERE LENGTH(Anagrama) = 4
GROUP BY Anagrama
HAVING COUNT(*) = 3 