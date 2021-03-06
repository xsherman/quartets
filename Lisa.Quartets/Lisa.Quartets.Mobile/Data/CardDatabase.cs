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
            _database.CreateTable<RequestCardStats>();
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

        public void ResetCards()
        {
            _database.Execute("UPDATE Card SET IsInHand = 0, IsQuartet = 0");
        }

        public void UpdateSelectedCards(List<int> ids)
        {
            ResetCards();

            string idString = string.Join(",", ids.Select(n => n.ToString()).ToArray());
            string query = string.Format("UPDATE 'Card' SET IsInHand = 1 WHERE Id in ({0})", idString);

            _database.Execute(query);
        }

        public void GiveCardAway(int id)
        {
            _database.Execute("UPDATE Card SET IsInHand = 0 WHERE Id = ?", id);
        }

        public List<Card> RetrieveCardsInHand()
        {
			return _database.Query<Card>("SELECT * FROM Card WHERE IsInHand = 1");
        }

        public List<Card> RetrieveNonQuartetCardsInHand()
        {
            return _database.Query<Card>("SELECT * FROM Card WHERE IsInHand = 1 AND IsQuartet = 0");
        }

        public bool IsQuartet(int category)
        {
            List<Card> cards = _database.Query<Card>("SELECT * FROM Card WHERE IsInHand = 1 AND Category =" + category);

            return cards.Count == 4;
        }

        public void SetQuartet(int category)
        {
            _database.Execute("UPDATE Card SET IsQuartet = 1 WHERE Category =" + category);
        }

        public List<int> RetrieveCategories()
        {
            List<int> categoryList = new List<int>();
            var cards = _database.Query<Card>("SELECT * FROM Card GROUP BY Category");
            foreach (Card card in cards)
            {
                categoryList.Add(card.Category);
            }

            return categoryList;
        }

        public List<Card> RetrieveQuartet(int category)
        {
            return _database.Query<Card>("SELECT * FROM Card WHERE Category =" + category);
        }

        public List<Card> RetrieveRequestableCards()
        {              
            return _database.Query<Card>("SELECT * FROM Card WHERE IsInHand = 0 AND Category in (SELECT Category FROM Card WHERE IsInHand = 1 GROUP By Category)");
        }

        public Card RetrieveFromNumber(string number)
        {
            List<Card> cards = _database.Query<Card>("SELECT * from Card Where Number = ?", number);

            return cards.FirstOrDefault<Card>();
        }

        public void SaveCardRequestStats(RequestCardStats stats)
        {
            _database.InsertWithChildren(stats);
        }

        public List<RequestCardStats> RetrieveStatistics()
        {
            return _database.GetAllWithChildren<RequestCardStats>();
        }

        public void DeleteStats()
        {
            _database.DeleteAll<RequestCardStats>();
        }

		public void CreateDefaultCards()			
		{
            int category = 1;
            int j = 1;

			for (int i = 1; i <= 36; i++)
			{
				var card = new Card();
                string number = category.ToString() + j.ToString();

                card.Number = int.Parse(number);

				card.Name = string.Format("card{0}", i);
                card.Category = category;
				card.FileName = string.Format("card{0}.png", i);
                card.SoundFile = string.Format("sound{0}", i);
				card.IsInHand = 0;
                card.IsQuartet = 0;
				CreateCard(card);

                j++;
                if (j >= 5)
                {
                    j = 1;
                    category++;
                }
			}
		}

		private SQLiteConnection _database;
	}
}