﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net;
using SQLiteNetExtensions.Extensions;
using Xamarin.Forms;

namespace Lisa.Quartets.Mobile
{
	public class CardDatabase
	{
		public CardDatabase()
		{
			_database = DependencyService.Get<ISQLite>().GetConnection();
			_database.CreateTable<Card>();
		}

		public void DeleteCards()
		{
			_database.DeleteAll<Card>();
		}

		public List<Card> RetrieveCards()
		{
			return _database.GetAllWithChildren<Card>();
		}

		public int CreateOrUpdateCard(Card card)
		{
			if (card.Id != 0)
			{
				_database.InsertOrReplaceWithChildren(card);
				return card.Id;
			}
			else
			{
				return _database.Insert(card);
			}
		}

		public int DeleteCard(int id)
		{
			return _database.Delete<Card>(id);
		}

		public void CreateCard(Card card)
		{
			_database.InsertWithChildren(card);
		}

		public Card RetrieveCard(int cardId)
		{
			return _database.GetWithChildren<Card>(cardId);
		}

		public void CreateDefaultCards()			
		{
			for (int i = 0; i < 16; i++)
			{
				var card = new Card();
				card.Name = string.Format("card{0}", i);
				card.Category = "test";
				card.FileName = string.Format("card{0}.jpg", i);
				card.IsInHand = false;

				CreateCard(card);
			}
		}

		private SQLiteConnection _database;
	}
}