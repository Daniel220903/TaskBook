using Bogus;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
//AQUI ESTAS PONIENDO LAS PRUEBAS NOMAS
namespace BookApp.Controllers
{
    public class tryController : Controller
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
        public IActionResult GenerateBooks(int page = 1)
        {
            Randomizer.Seed = new Random(CurrentSeed + page);
            var faker = new Faker(CurrentLanguage);

            var books = new List<dynamic>();

            int roundedLikesFloor = 0;
            int roundedLikesCeiling = 0;

            if (AverageLikes % 1 == 0)
            {
                roundedLikesFloor = roundedLikesCeiling = (int)AverageLikes;
            }
            else
            {
                roundedLikesFloor = (int)Math.Floor(AverageLikes);
                roundedLikesCeiling = (int)Math.Ceiling(AverageLikes);
            }

            int roundedReviewsFloor = 0;
            int roundedReviewsCeiling = 0;
            if (AverageReviews % 1 == 0)
            {
                roundedReviewsFloor = roundedReviewsCeiling = (int)AverageReviews;
            }
            else
            {
                roundedReviewsFloor = (int)Math.Floor(AverageReviews);
                roundedReviewsCeiling = (int)Math.Ceiling(AverageReviews);
            }

            for (int i = 1; i <= 10; i++)
            {
                var reviewsList = new List<string>();

                int reviewsIterations = roundedReviewsFloor == roundedReviewsCeiling
                    ? roundedReviewsFloor
                    : new Random().Next(0, 2) == 0
                        ? roundedReviewsFloor
                        : roundedReviewsCeiling;

                for (int j = 0; j < reviewsIterations; j++){
                    string reviewText = faker.Lorem.Sentence(10);
                    string reviewAuthor = faker.Name.FullName();

                    reviewsList.Add($"Review by {reviewAuthor}: <br>{reviewText}");
                }

                int likesIterations = roundedLikesFloor == roundedLikesCeiling
                    ? roundedLikesFloor
                    : new Random().Next(0, 2) == 0
                        ? roundedLikesFloor
                        : roundedLikesCeiling;

                int index = (page - 1) * 10 + i;
                books.Add(new
                {
                    Index = index,
                    ISBN = faker.Random.Replace("###-#-##-######-#"),
                    Title = faker.Hacker.Adjective() + " " + faker.Vehicle.Manufacturer(),
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


        [HttpPost]
        public IActionResult UpdateSettings(string language, int seed, decimal likes, decimal reviews)
        {
            CurrentLanguage = language;
            CurrentSeed = seed;
            AverageLikes = likes;
            AverageReviews = reviews;

            return Json(new { success = true });
        }
    }
}