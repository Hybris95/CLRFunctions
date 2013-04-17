using System;
using Microsoft.SqlServer.Server;
using System.Text.RegularExpressions;
using System.Collections;
using System.Data.SqlTypes;

namespace Deltadore.DotNet.Cbuisson
{
	/// <summary>
	/// Cette classe est utilis�e pour mettre � disposition une librairie de fonctions d'Expressions R�guli�res destin�es pour SQL Server 2005 (et plus) en CLR
	/// </summary>
	public class RegularExpressionFunctions
	{
		/**
		<summary>
		Cette m�thode permet de r�cup�rer toutes les sous-chaines d'une chaine correspondant � une expression r�guli�re (sous forme de chaine concat�n�e)
		</summary>
		<param name="pattern">
		Cette cha�ne de caract�res repr�sente l'expression r�guli�re � comparer
		</param>
		<param name="sentence">
		Cette cha�ne de caract�res repr�sente l'expression � �valuer
		</param>
		<param name="separator">
		Cette cha�ne de caract�res sera ins�r�e entre chaque sous-cha�ne
		</param>
		<returns>
		Soit null si aucune sous-chaine ne correspond, soit une chaine correspondant � la concat�nation des diff�rentes sous-cha�nes s�par�es par une cha�ne de s�paration
		</returns>
		*/
		[SqlFunction(IsDeterministic = true, IsPrecise = true, Name = "RegExMatches")]
		public static String RegExMatches(String pattern, String sentence, String separator)
		{
			Regex rgx = new Regex(pattern);
			MatchCollection matches = rgx.Matches(sentence);
			int nbFound = matches.Count;
			if(nbFound == 0){return null;}// Retourne null si aucune sous-cha�ne ne correspond � l'expression r�guli�re
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
			return toReturn;// Retourne les sous-cha�nes s�par�es par la cha�ne de s�paration
		}
		
		/**
		<summary>
		Cette m�thode permet de r�cup�rer toutes les sous-chaines d'une chaine correspondant � une expression r�guli�re (sous forme de tableau)
		</summary>
		<param name="pattern">
		Cette cha�ne de caract�res repr�sente l'expression r�guli�re � comparer
		</param>
		<param name="sentence">
		Cette cha�ne de caract�res repr�sente l'expression � �valuer
		</param>
		<returns>
		Un tableau de taille �gale au nombre de sous-cha�ne trouv�es, contenant dans chaque case une sous-cha�ne correspondant � l'expression r�guli�re
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
			return toReturn;// Retourne les sous-cha�nes dans un tableau
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
		Cette m�thode permet de r�cup�rer le nombre de sous-cha�nes d'une cha�ne correspondant � une expression r�guli�re
		</summary>
		<param name="pattern">
		Cette cha�ne de caract�res repr�sente l'expression r�guli�re � comparer
		</param>
		<param name="sentence">
		Cette cha�ne de caract�res repr�sente l'expression � �valuer
		</param>
		<returns>
		Le nombre d'occurences des sous-cha�nes trouv�e par l'expression r�guli�re dans la cha�ne d'entr�e
		</returns>
		*/
		[SqlFunction(IsDeterministic = true, IsPrecise = true, Name = "RegExNbMatches")]
		public static int RegExNbMatches(String pattern, String sentence)
		{
			return new Regex(pattern).Matches(sentence).Count;// Retourne le nombre de sous-cha�nes trouv�es
		}
		
		/**
		<summary>
		Cette m�thode permet de savoir si une cha�ne de caract�re correspond � un pattern d'expression r�guli�re
		</summary>
		<param name="pattern">
		Cette cha�ne de caract�res repr�sente l'expression r�guli�re � comparer
		</param>
		<param name="sentence">
		Cette cha�ne de caract�res repr�sente l'expression � �valuer
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