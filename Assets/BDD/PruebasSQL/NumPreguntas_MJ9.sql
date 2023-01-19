SELECT FrasesTexto.ID_Frase, Frase, count(*) AS NumPreguntas
FROM FrasesTexto 
INNER JOIN Preguntas 
ON FrasesTexto.ID_Frase = Preguntas.ID_Frase 
INNER JOIN Dificultad
ON FrasesTexto.ID_Frase = Dificultad.ID_Entrada
WHERE Dificultad.Identificador = 'Frase' AND Dificultad.Minijuego9 = 1
GROUP BY FrasesTexto.ID_Frase
HAVING NumPreguntas >= 1