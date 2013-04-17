using System;
using Microsoft.SqlServer.Server;
using System.Text.RegularExpressions;
using System.Collections;
using System.Data.SqlTypes;

namespace Deltadore.DotNet.Cbuisson
{
	/// <summary>
	/// Cette classe est utilisée pour mettre à disposition une librairie de fonctions d'Expressions Régulières destinées pour SQL Server 2005 (et plus) en CLR
	/// </summary>
	public class RegularExpressionFunctions
	{
		/**
		<summary>
		Cette méthode permet de récupérer toutes les sous-chaines d'une chaine correspondant à une expression régulière (sous forme de chaine concaténée)
		</summary>
		<param name="pattern">
		Cette chaîne de caractères représente l'expression régulière à comparer
		</param>
		<param name="sentence">
		Cette chaîne de caractères représente l'expression à évaluer
		</param>
		<param name="separator">
		Cette chaîne de caractères sera insérée entre chaque sous-chaîne
		</param>
		<returns>
		Soit null si aucune sous-chaine ne correspond, soit une chaine correspondant à la concaténation des différentes sous-chaînes séparées par une chaîne de séparation
		</returns>
		*/
		[SqlFunction(IsDeterministic = true, IsPrecise = true, Name = "RegExMatches")]
		public static String RegExMatches(String pattern, String sentence, String separator)
		{
			Regex rgx = new Regex(pattern);
			MatchCollection matches = rgx.Matches(sentence);
			int nbFound = matches.Count;
			if(nbFound == 0){return null;}// Retourne null si aucune sous-chaîne ne correspond à l'expression régulière
			String toReturn = "";
			for(int i = 0; i < nbFound; i++)
			{
				Match match = matches[i];
				if(i != 0)
				{
					toReturn += separator;
				}
				toReturn += match.Value;
			}
			return toReturn;// Retourne les sous-chaînes séparées par la chaîne de séparation
		}
		
		/**
		<summary>
		Cette méthode permet de récupérer toutes les sous-chaines d'une chaine correspondant à une expression régulière (sous forme de tableau)
		</summary>
		<param name="pattern">
		Cette chaîne de caractères représente l'expression régulière à comparer
		</param>
		<param name="sentence">
		Cette chaîne de caractères représente l'expression à évaluer
		</param>
		<returns>
		Un tableau de taille égale au nombre de sous-chaîne trouvées, contenant dans chaque case une sous-chaîne correspondant à l'expression régulière
		</returns>
		*/
		[SqlFunction(Name = "RegExMatchesSplit", FillRowMethodName = "NextSplitRow", DataAccess = DataAccessKind.Read)]
		public static IEnumerable RegExMatchesSplit(String pattern, String sentence)
		{
			Regex rgx = new Regex(pattern);
			MatchCollection matches = rgx.Matches(sentence);
			int nbFound = matches.Count;
			//String[][] toReturn = new String[nbFound][];
			String[] toReturn = new String[nbFound];
			for(int i = 0; i < nbFound; i++)
			{
				/*toReturn[i] = new String[2];
				toReturn[i][0] = sentence;
				toReturn[i][1] = matches[i].Value;*/
				toReturn[i] = matches[i].Value;
			}
			return toReturn;// Retourne les sous-chaînes dans un tableau
		}
		public static void NextSplitRow(Object obj, /*out SqlString sentence, */out SqlString match)
		{
			/*String[] row = (String[])obj;
			sentence = new SqlString(row[0]);
			match = new SqlString(row[1]);*/
			match = new SqlString(obj.ToString());
		}
		
		/**
		<summary>
		Cette méthode permet de récupérer le nombre de sous-chaînes d'une chaîne correspondant à une expression régulière
		</summary>
		<param name="pattern">
		Cette chaîne de caractères représente l'expression régulière à comparer
		</param>
		<param name="sentence">
		Cette chaîne de caractères représente l'expression à évaluer
		</param>
		<returns>
		Le nombre d'occurences des sous-chaînes trouvée par l'expression régulière dans la chaîne d'entrée
		</returns>
		*/
		[SqlFunction(IsDeterministic = true, IsPrecise = true, Name = "RegExNbMatches")]
		public static int RegExNbMatches(String pattern, String sentence)
		{
			return new Regex(pattern).Matches(sentence).Count;// Retourne le nombre de sous-chaînes trouvées
		}
		
		/**
		<summary>
		Cette méthode permet de savoir si une chaîne de caractère correspond à un pattern d'expression régulière
		</summary>
		<param name="pattern">
		Cette chaîne de caractères représente l'expression régulière à comparer
		</param>
		<param name="sentence">
		Cette chaîne de caractères représente l'expression à évaluer
		</param>
		<returns>
		True si l'expression correspond bien au pattern, false dans le cas contraire
		</returns>
		*/
		[SqlFunction(IsDeterministic = true, IsPrecise = true, Name = "RegExIsMatch")]
		public static bool RegExIsMatch(String pattern, String sentence)
		{
			return new Regex(pattern).IsMatch(sentence);
		}
		
		/**
		TODO - Documentation
		*/
		[SqlFunction(IsDeterministic = true, IsPrecise = true, Name = "RegExMatch")]
		public static String RegExMatch(String pattern, String sentence)
		{
			Match match = new Regex(pattern).Match(sentence);
			if(!match.Success){return null;}
			return match.Value;
		}
	}
}