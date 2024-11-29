using Microsoft.AspNetCore.Mvc;
using Bogus;
using System.Collections.Generic;
using AutoFixture;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Bogus.DataSets;

namespace BookApp.Controllers
{
    public class BookController : Controller
    {
        private static string CurrentLanguage = "en";
        private static int CurrentSeed = 42;
        private static decimal AverageLikes = 3.7m;
        private static decimal AverageReviews = 2.5m;

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GenerateBooks(int page = 1) {
            var books = new List<dynamic>();

            int roundedLikesFloor = (int)Math.Floor(AverageLikes);
            int roundedLikesCeiling = (int)Math.Ceiling(AverageLikes);

            int roundedReviewsFloor = (int)Math.Floor(AverageReviews);
            int roundedReviewsCeiling = (int)Math.Ceiling(AverageReviews);

            for (int i = 1; i <= 10; i++){
                var reviewsList = new List<string>();

                int likesIterations = roundedLikesFloor == roundedLikesCeiling
                    ? roundedLikesFloor
                    : new Random(CurrentSeed + i).Next(0, 2) == 0
                        ? roundedLikesFloor
                        : roundedLikesCeiling;

                int reviewsIterations = roundedReviewsFloor == roundedReviewsCeiling
                    ? roundedReviewsFloor
                    : new Random(CurrentSeed + i * 100).Next(0, 2) == 0
                        ? roundedReviewsFloor
                        : roundedReviewsCeiling;

                for (int j = 0; j < reviewsIterations; j++){
                    Randomizer.Seed = new Random(CurrentSeed + (i * 1000) + j);
                    string resultado = GenerateReview();
                    reviewsList.Add(resultado);
                }

                Randomizer.Seed = new Random(CurrentSeed + (page * 100) + i);
                var faker = new Faker(CurrentLanguage);

                int index = (page - 1) * 10 + i;
                books.Add(new {
                    Index = index,
                    ISBN = faker.Random.Replace("###-#-##-######-#"),
                    Title = faker.Lorem.Sentence(),
                    Author = faker.Name.FullName(),
                    Publisher = faker.Company.CompanyName(),
                    Likes = likesIterations,
                    Reviews = reviewsIterations,
                    ReviewList = reviewsList,
                    CoverImage = faker.Image.LoremFlickrUrl()
                });
            }

            return Json(books);
        }

        private string GenerateReview(){
            var faker = new Faker(CurrentLanguage);
            string reviewText = faker.Lorem.Sentence(10);
            string reviewAuthor = faker.Name.FullName();
            return $"Review by {reviewAuthor}: <br>{reviewText}";
        }

        [HttpPost]
        public IActionResult UpdateSettings(string language, int seed, decimal likes, decimal reviews){
            CurrentLanguage = language;
            CurrentSeed = seed;
            AverageLikes = likes;
            AverageReviews = reviews;

            return Json(new { success = true });
        }
    }
}