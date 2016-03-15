﻿using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace Lisa.Quartets.Mobile
{
	public partial class HandEditorView : ContentPage
	{
        public HandEditorView()
        {
            InitializeComponent();
            SetPreviousSelectedCards();
            SetImages();
        }

        private void SaveSelectedCards(object sender, EventArgs args)
        {
            _database.UpdateSelectedCards(_selectedImages);

			// REVIEW: Is it safe to call an async function in a method that isn't async itself?
            Navigation.PopToRootAsync();
        }

        private void StatsButtonClicked(object sender, EventArgs args)
        {
            Navigation.PushAsync(new StatisticView());
        }

		private void OnImageClick(object sender, EventArgs args)
        {
            var image = (CardImage) sender;

			if (IsSelected(image))
			{
                Deselect(image);
                image.ScaleTo(0.8, 100);

                if (IsIos()) {
					image.FadeTo(0.5, 100);   
				}
                else
                {					
					_opacity[image.CardId].FadeTo(1, 100);
				}
			}
			else
			{
                Select(image);
                image.ScaleTo(1, 100);

                if (IsIos()) {
					image.FadeTo(1, 100);   
				}
                else
                {
					_opacity[image.CardId].FadeTo(0, 100);
				}

			}

            saveButton.IsEnabled = true;
		}

        private void SetImages()
        {
            CardLayout cardGrid = new CardLayout();
            List<Card> cards = _database.RetrieveCards();

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += OnImageClick;
            FileImageSource opacitySource = new FileImageSource { File = "opacity.png" };

            foreach (var card in cards)
            {
                CardImage image = new CardImage();
                image.Source = card.FileName;
                image.CardId = card.Id;

                image.Scale = 0.8;
                image.GestureRecognizers.Add(tapGestureRecognizer);
                //Image shadow = new Image { Source = shawdowSource, Scale = 0.8 };

                if (IsIos()) {
					if (IsSelected(image)) {
						image.Opacity = 1;
						image.Scale = 1;
					} else {
						image.Opacity = 0.5;
						image.Scale = 0.8;
					}
					
					cardGrid.Children.Add(image);				
				} else {
					Image opacity = new Image { Source = opacitySource, Scale = 0.8 };

					if (IsSelected(image))
					{
						opacity.Opacity = 0;
						image.Scale = 1;
					}

					_opacity.Add(image.CardId, opacity);

                    var parent = new AbsoluteLayout{ Children = { image, _opacity[image.CardId] } };

					cardGrid.Children.Add(parent);
				}
            }

            scrollView.Content = cardGrid;
        }

        private bool IsSelected(CardImage image)
        { 
            return _selectedImages.Contains(image.CardId);
        }

        private bool IsIos()
        {
            return Device.OS == TargetPlatform.iOS;
        }

        private void Select(CardImage image)
        {
            _selectedImages.Add(image.CardId);
        }

        private void Deselect(CardImage image)
        {
            
            _selectedImages.Remove(image.CardId);
        }

        private void SetPreviousSelectedCards()
        {
            foreach (Card card in _database.RetrieveCardsWhereInHandIs(1))
            {
                _selectedImages.Add(card.Id);
            }
        }

		private CardDatabase _database = new CardDatabase();
        private List<int> _selectedImages = new List<int>();
		private Dictionary<int, Image> _opacity = new Dictionary<int, Image>();
//      private CardImageHolder _cardImageHolder;
	}
}