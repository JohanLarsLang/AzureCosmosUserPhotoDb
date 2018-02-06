//Lab 4 Databas Azure 

//Johan Lång

//Göteborg: 180205

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using System.Threading;
using Microsoft.Azure.WebJobs.Host;
using System.Text.RegularExpressions;

namespace databaslab4
{
    public class Program
    {
        private const string EndpointUrl = "https://databaslab4db.documents.azure.com:443/";
        private const string PrimaryKey = "eB5xtIsl9nbuZEpx8xO7twriGpjUntYebVC5KixEOKffN4GYvWgO5Osm7yO1m9DVczw8RY6FXCbet85OcXC0Vw==";
        private DocumentClient client;

        static void Main(string[] args)
        {
            try
            {
                Program p = new Program();

                p.UserListMenu().Wait();

                // Start s = new Start();
                //s.GetStartedLabFour().Wait();
            }
            catch (DocumentClientException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}, Message: {2}", de.StatusCode, de.Message, baseException.Message);
            }
            catch (Exception e)
            {
                Exception baseException = e.GetBaseException();
                Console.WriteLine("Error: {0}, Message: {1}", e.Message, baseException.Message);
            }
            finally
            {
                Console.WriteLine($"\nEnd of program, press any key to exit.");
                Console.ReadKey();
            }

        }

        private async Task UserListMenu()
        {
            this.client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);
            await this.client.CreateDatabaseIfNotExistsAsync(new Database { Id = "DatabasLab4DB" });

            Console.Clear();
            Console.WriteLine($"****** User and Photo Database *******\n");
            Console.WriteLine($"All Users in the database:\n");

            this.QueryUserList("DatabasLab4DB", "UsersCollection");
        }

        private async Task PhotoListMenu()
        {
            this.client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);
            await this.client.CreateDatabaseIfNotExistsAsync(new Database { Id = "DatabasLab4DB" });

            Console.Clear();
            Console.WriteLine($"****** User and Photo Database *******\n");
            Console.WriteLine($"All Photos to be approved:\n");

            this.QueryPhotoCollection("DatabasLab4DB", "VerifyPhotosCollection");
        }

        private async Task PhotoApprovedListMenu()
        {
            this.client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);
            await this.client.CreateDatabaseIfNotExistsAsync(new Database { Id = "DatabasLab4DB" });

            Console.Clear();
            Console.WriteLine($"****** User and Photo Database *******\n");
            Console.WriteLine($"All approved Photos:\n");

            this.QueryPhotoApprovedCollection("DatabasLab4DB", "ApprovedPhotosCollection");
        }

        /*
        private async Task GetStartedLabFour()
        {

            this.client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);

            await this.client.CreateDatabaseIfNotExistsAsync(new Database { Id = "DatabasLab4DB" });

            await this.client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri("DatabasLab4DB"), new DocumentCollection { Id = "UsersCollection" });

            await this.client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri("DatabasLab4DB"), new DocumentCollection { Id = "VerifyPhotosCollection" });

            await this.client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri("DatabasLab4DB"), new DocumentCollection { Id = "ApprovedPhotosCollection" });

            await this.DeleteDocument("DatabasLab4DB", "UsersCollection", "User.6");
            //await this.DeleteDocument("DatabasLab4DB", "ApprovedPhotosCollection", "Photo.5");

            /*
            Photo photo1 = new Photo
            {
                Id = "Photo.5",
                PhotoUrl = "http://www.fnordware.com/superpng/pnggrad16rgb.png",
                IsSelected = false,
                IsApproved = false
            };

            await this.CreatePhotoDocumentIfNotExists("DatabasLab4DB", "VerifyPhotosCollection", photo1);

            Photo photo2 = new Photo
            {
                Id = "Photo.6",
                PhotoUrl = "http://www.fnordware.com/superpng/pnggrad16rgb.png",
                IsSelected = false,
                IsApproved = false
            };

            await this.CreatePhotoDocumentIfNotExists("DatabasLab4DB", "VerifyPhotosCollection", photo2);


            Photo johanPhoto = new Photo
            {
                Id = "Photo.1",
                PhotoUrl = "http://www.fnordware.com/superpng/pnggrad16rgb.png",
                IsSelected = true,
                IsApproved = true
            };

            // await this.CreatePhotoDocumentIfNotExists("DatabasLab4DB", "ApprovedPhotosCollection", johanPhoto);

            User johanUser = new User
            {
                Id = "User.1",
                Name = "Johan",
                Email = "johan@email.se",
                PhotoId = "Photo.1",
                PhotoUrl = "http://www.fnordware.com/superpng/pnggrad16rgb.png",
                IsApproved = true
            };

            //await this.CreateUserDocumentIfNotExists("DatabasLab4DB", "UsersCollection", johanUser);

            Photo tommyPhoto = new Photo
            {
                Id = "Photo.2",
                PhotoUrl = "http://www.fnordware.com/superpng/pngtest8rgba.png",
                IsSelected = true,
                IsApproved = true
            };

            //await this.CreatePhotoDocumentIfNotExists("DatabasLab4DB", "ApprovedPhotosCollection", tommyPhoto);

            User tommyUser = new User
            {
                Id = "User.2",
                Name = "Tommy",
                Email = "tommy@email.se",
                PhotoId = "Photo.2",
                PhotoUrl = "http://www.fnordware.com/superpng/pngtest8rgba.png",
                IsApproved = true
            };

            //await this.CreateUserDocumentIfNotExists("DatabasLab4DB", "UsersCollection", tommyUser);

            // this.QueryUserList("DatabasLab4DB", "UsersCollection");

            // this.QueryPhotoCollection("DatabasLab4DB", "VerifyPhotosCollection");
            
        }
    */

        private void QueryUserList(string databaseName, string collectionName)
        {
            // Set some common query options
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };

            IQueryable<User> userQuery = this.client.CreateDocumentQuery<User>(
                  UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), queryOptions);

            foreach (User user in userQuery)
            {
                Console.WriteLine($"Id: {user.Id} Name: {user.Name} Email: {user.Email} PhotoId: {user.PhotoId} PhotoUrl: {user.PhotoUrl}");
            }

            Console.WriteLine($"\n[A] Add new user");
            Console.WriteLine($"[P] Photos approved");
            Console.WriteLine($"[V] Verify photos to approval");
            Console.WriteLine($"    Any other key to quit");

            var key = Console.ReadKey();

            if (key.Key == ConsoleKey.A)
            {
                AddUser().Wait();
            }

            else if (key.Key == ConsoleKey.P)
            {
                PhotoApprovedListMenu().Wait();
            }

            else if (key.Key == ConsoleKey.V)
            {
                PhotoListMenu().Wait();
            }

        }

        private void QueryPhotoCollection(string databaseName, string collectionName)
        {
            // Set some common query options
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };

            IQueryable<Photo> photoQuery = this.client.CreateDocumentQuery<Photo>(
                   UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), queryOptions);

            foreach (Photo photo in photoQuery)
            {
                Console.WriteLine($"Id: {photo.Id} PhotoUrl: {photo.PhotoUrl} IsApproval: {photo.IsApproved}");
            }

            Console.WriteLine($"\n[A] Approve photo");
            Console.WriteLine($"[D] Delete photo");
            Console.WriteLine($"[N] New photo");
            Console.WriteLine($"[Q] Quit");

            var key = Console.ReadKey();

            if (key.Key == ConsoleKey.A)
            {
                ApprovePhoto().Wait();
            }

            else if (key.Key == ConsoleKey.D)
            {
                bool verifyPhotoExist = CountPhotos("DatabasLab4DB", "VerifyPhotosCollection");

                if (verifyPhotoExist == true)
                    DeletePhoto().Wait();

                else
                {
                    Console.WriteLine($"\nThere are no photos to delete, pressany key to quit");
                    Console.ReadKey();
                    PhotoListMenu().Wait();
                }

            }

            else if (key.Key == ConsoleKey.N)
            {
                NewPhoto().Wait();
            }

            else if (key.Key == ConsoleKey.Q)
            {
                UserListMenu().Wait();
            }

            else
            {
                PhotoListMenu().Wait();
            }
        }

        private void QueryPhotoApprovedCollection(string databaseName, string collectionName)
        {
            // Set some common query options
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };

            IQueryable<Photo> photoQuery = this.client.CreateDocumentQuery<Photo>(
                   UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), queryOptions);

            foreach (Photo photo in photoQuery)
            {
                Console.WriteLine($"Id: {photo.Id} PhotoUrl: {photo.PhotoUrl} IsApproval: {photo.IsApproved} IsSelected: {photo.IsSelected}");
            }

            Console.WriteLine($"\nPress any key to quit");

            Console.ReadKey();
            UserListMenu().Wait();

        }

        private void WriteToConsoleAndPromptToContinue(string format, params object[] args)
        {
            Console.WriteLine(format, args);
            Console.WriteLine("Press any key to continue ...");
            Console.ReadKey();
        }


        private async Task CreateUserDocumentIfNotExists(string databaseName, string collectionName, User user)
        {
            try
            {
                await this.client.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, user.Id));
                this.WriteToConsoleAndPromptToContinue("Found {0}", user.Id);
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), user);
                    this.WriteToConsoleAndPromptToContinue("Created User {0}", user.Id);
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task CreatePhotoDocumentIfNotExists(string databaseName, string collectionName, Photo photo)
        {
            try
            {
                await this.client.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, photo.Id));
                this.WriteToConsoleAndPromptToContinue("Found {0}", photo.Id);
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), photo);
                    this.WriteToConsoleAndPromptToContinue("Created Photo {0}", photo.Id);
                }
                else
                {
                    throw;
                }
            }
        }


        private async Task DeleteDocument(string databaseName, string collectionName, string documentName)
        {
            await this.client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, documentName));
            Console.WriteLine("Deleted Document {0}", documentName);
        }


        private int CountNextId(string databaseName, string collectionName, string value)
        {
            // Set some common query options
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };

            string nextIdStr = "";
            int nextId = 0;

            if (value == "User")
            {
                IQueryable<User> query = this.client.CreateDocumentQuery<User>(
                 UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), queryOptions);

                var queryId = from x in query
                              select x.Id;

                foreach (var x in queryId)
                    nextIdStr = $"{x}";

                string toBeSearched = "User.";
                nextIdStr = nextIdStr.Substring(nextIdStr.IndexOf(toBeSearched) + toBeSearched.Length);

                nextId = int.Parse(nextIdStr);
            }

            else if (value == "Photo")
            {
                IQueryable<Photo> query = this.client.CreateDocumentQuery<Photo>(
                 UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), queryOptions);

                int count = query.Count();

                if (count > 0)
                {
                    var queryId = from x in query
                                  select x.Id;

                    int photoIdNr = 0;

                    foreach (var p in queryId)
                    {
                        string nextPhotoIdStr = p.Split('.')[1];
                        photoIdNr = int.Parse(nextPhotoIdStr);

                        if (photoIdNr > nextId)
                            nextId = photoIdNr;

                    }

                }
            }

            nextId++;
            return nextId;

        }

        private bool CountApprovedNotSelectedPhoto(string databaseName, string collectionName)
        {
            // Set some common query options
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };

            IQueryable<Photo> photoQuery = this.client.CreateDocumentQuery<Photo>(
                  UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), queryOptions);

            var query = from photo in photoQuery
                        where photo.IsSelected == false
                        select photo;

            int count = query.Count();

            if (count > 0)
                return true;

            else
                return false;

        }

        private bool CountPhotos(string databaseName, string collectionName)
        {
            // Set some common query options
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };

            IQueryable<Photo> photoQuery = this.client.CreateDocumentQuery<Photo>(
                  UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), queryOptions);

            var query = from photo in photoQuery
                        select photo;

            int count = query.Count();

            if (count > 0)
                return true;

            else
                return false;

        }

        private string GetAllPhotoId(string databaseName, string collectionName, string value)
        {
            // Set some common query options
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };

            IQueryable<Photo> photoQuery = this.client.CreateDocumentQuery<Photo>(
                  UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), queryOptions);

            IQueryable<string> queryid = null;

            if (value == "All")
            {
                queryid = from photo in photoQuery
                          select photo.Id;

            }
            else if (value == "NotSelected")
            {
                queryid = from photo in photoQuery
                          where photo.IsSelected == false
                          select photo.Id;
            }
            string allPhotoId = "";

            foreach (var x in queryid)
                allPhotoId += " " + x;

            return allPhotoId;

        }

        private int CheckName(string name)
        {
            string patternName = @"\A([A-Z]|[ÅÄÖ])\w{1,}";
            var matchesName = Regex.Matches(name, patternName);
            int nrMatchName = matchesName.Count;
            return nrMatchName;
        }

        private int CheckEmail(string email)
        {
            string patternEmail = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";

            var matchesEmail = Regex.Matches(email, patternEmail);
            int nrMatchEmail = matchesEmail.Count;

            return nrMatchEmail;
        }

        private int CheckUrl(string url)
        {

            string patternUrl = @"(?i)\.(jpg|png|gif)$";

            var matchesUrl = Regex.Matches(url, patternUrl);
            int nrMatchUrl = matchesUrl.Count;
            return nrMatchUrl;
        }


        private async Task AddUser()
        {
            bool createUser = CountApprovedNotSelectedPhoto("DatabasLab4DB", "ApprovedPhotosCollection");

            if (createUser == false)
            {
                Console.WriteLine($"No approved photos available, press any key to quit");
                Console.ReadKey();
            }

            else
            {
                bool loop = false;
                string id = "";

                while (loop != true)
                {
                    string allApprovedPhotoId = GetAllPhotoId("DatabasLab4DB", "ApprovedPhotosCollection", "NotSelected");
                    Console.WriteLine($"\nApproved PhotoId for select: {allApprovedPhotoId}");
                    Console.WriteLine($"\nEnter valid PhotoId for select, quit with Q: ");
                    id = Console.ReadLine();

                    bool checkId = allApprovedPhotoId.Contains(id);

                    try
                    {
                        if (id == "q" || id == "Q")
                        {
                            UserListMenu().Wait();
                            loop = true;
                        }


                        else if (checkId == true)
                        {
                            bool loop2 = false;
                            string name = "";

                            while (loop2 != true)
                            {
                                Console.WriteLine($"\nEnter name (must start with a capital letter): ");
                                name = Console.ReadLine();
                                int nrMatchName = CheckName(name);

                                try
                                {
                                    if (nrMatchName >= 1)
                                        loop2 = true;
                                }

                                catch
                                {
                                    loop2 = false;
                                }
                            }

                            loop2 = false;
                            string email = "";

                            while (loop2 != true)
                            {
                                Console.WriteLine($"\nEnter a vaild email: ");
                                email = Console.ReadLine();
                                int nrMatchEmail = CheckEmail(email);

                                try
                                {
                                    if (nrMatchEmail >= 1)
                                        loop2 = true;
                                }

                                catch
                                {
                                    loop2 = false;
                                }
                            }

                            int userid = CountNextId("DatabasLab4DB", "UsersCollection", "User");
                            string photoUrl = GetPhotoUrl("DatabasLab4DB", "ApprovedPhotosCollection", id);
                            User user = new User
                            {
                                Id = $"User.{userid}",
                                Name = name,
                                Email = email,
                                PhotoId = id,
                                PhotoUrl = photoUrl,
                                IsApproved = true
                            };

                            await this.CreateUserDocumentIfNotExists("DatabasLab4DB", "UsersCollection", user);

                            Photo updatePhoto = new Photo
                            {
                                Id = id,
                                PhotoUrl = photoUrl,
                                IsSelected = true,
                                IsApproved = true
                            };

                            await this.ReplacePhotoDocument("DatabasLab4DB", "ApprovedPhotosCollection", @id, updatePhoto);

                            loop = true;
                        }
                    }

                    catch
                    {
                        loop = false;
                    }
                }

                UserListMenu().Wait();
            }
        }
        private async Task NewPhoto()
        {
            bool loop = false;
            string photoUrl = "";

            int nextPhotoId = 0;

            int approvNextPhotoid = CountNextId("DatabasLab4DB", "ApprovedPhotosCollection", "Photo");
            //Console.WriteLine(approvNextPhotoid);

            int verifyNextPhotoid = CountNextId("DatabasLab4DB", "VerifyPhotosCollection", "Photo");
            // Console.WriteLine(verifyNextPhotoid);

            if (verifyNextPhotoid > approvNextPhotoid)
                nextPhotoId = verifyNextPhotoid;
            else
                nextPhotoId = approvNextPhotoid;

            //Console.WriteLine(nextPhotoId);


            while (loop != true)
            {
                Console.WriteLine($"\nEnter valid PhotoUrl, quit with Q: ");
                photoUrl = Console.ReadLine();
                int nrMatchUrl = CheckUrl(photoUrl);

                try
                {
                    if (photoUrl == "q" || photoUrl == "Q")
                    {
                        PhotoListMenu().Wait();
                        loop = true;
                    }

                    else
                    {
                        if (nrMatchUrl >= 1)
                        {


                            Photo addPhoto = new Photo
                            {
                                Id = $"Photo.{nextPhotoId}",
                                PhotoUrl = photoUrl,
                                IsSelected = false,
                                IsApproved = false
                            };

                            await this.CreatePhotoDocumentIfNotExists("DatabasLab4DB", "VerifyPhotosCollection", addPhoto);
                            loop = true;
                        }

                    }
                }

                catch
                {
                    loop = false;
                }

            }

            PhotoListMenu().Wait();
        }


        private async Task DeletePhoto()
        {
            string photoid = GetAllPhotoId("DatabasLab4DB", "VerifyPhotosCollection", "All");

            bool loop = false;
            string id = "";

            while (loop != true)
            {
                Console.WriteLine($"\nEnter valid PhotoId for delete, quit with Q: ");
                id = Console.ReadLine();
                bool checkId = photoid.Contains(id);

                try
                {
                    if (id == "q" || id == "Q")
                    {
                        PhotoListMenu().Wait();
                        loop = true;
                    }


                    else if (checkId == true)
                    {
                        await this.DeleteDocument("DatabasLab4DB", "VerifyPhotosCollection", @id);
                        loop = true;
                    }
                }

                catch
                {
                    loop = false;
                }
            }

            PhotoListMenu().Wait();

        }

        private async Task ApprovePhoto()
        {
            string photoid = GetAllPhotoId("DatabasLab4DB", "VerifyPhotosCollection", "All");
            Console.WriteLine($"\nPhotoId to be approved: {photoid}");
            bool loop = false;
            string id = "";

            while (loop != true)
            {
                Console.WriteLine($"\nEnter valid PhotoId to approve, quit with Q: ");
                id = Console.ReadLine();
                bool checkId = photoid.Contains(id);
                // Console.WriteLine(checkId);
                try
                {
                    if (id == "q" || id == "Q")
                    {
                        PhotoListMenu().Wait();
                        loop = true;
                    }


                    else if (checkId == true)
                    {
                        string photoUrl = GetPhotoUrl("DatabasLab4DB", "VerifyPhotosCollection", id);
                        //Console.WriteLine(photoUrl);
                        Photo photo = new Photo
                        {
                            Id = id,
                            PhotoUrl = photoUrl,
                            IsSelected = false,
                            IsApproved = true
                        };

                        await this.CreatePhotoDocumentIfNotExists("DatabasLab4DB", "ApprovedPhotosCollection", photo);

                        await this.DeleteDocument("DatabasLab4DB", "VerifyPhotosCollection", @id);
                        loop = true;
                    }
                }

                catch
                {
                    loop = false;
                }
            }

            PhotoListMenu().Wait();

        }

        public string GetPhotoUrl(string databaseName, string collectionName, string photoId)
        {
            this.client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);
            // Set some common query options
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };

            IQueryable<Photo> photoQuery = this.client.CreateDocumentQuery<Photo>(
                  UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), queryOptions);

            var query = from p in photoQuery
                        where p.Id == photoId
                        select p.PhotoUrl;

            string photoUrl = "";

            foreach (var photo in query)
                photoUrl = $"{photo}";

            return photoUrl;
        }

        private async Task ReplacePhotoDocument(string databaseName, string collectionName, string photoName, Photo updatedPhoto)
        {
            await this.client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, photoName), updatedPhoto);
            this.WriteToConsoleAndPromptToContinue("Replaced Photo {0}", photoName);
        }
    }
}






