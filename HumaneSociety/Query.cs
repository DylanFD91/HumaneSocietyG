﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {        
        static HumanceSocietyDataContext db;

        static Query()
        {
            db = new HumanceSocietyDataContext();
        }

        internal static List<USState> GetStates()
        {
            List<USState> allStates = db.USStates.ToList();       

            return allStates;
        }
            
        internal static Client GetClient(string userName, string password)
        {
            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.City = null;
                newAddress.USStateId = stateId;
                newAddress.Zipcode = zipCode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
            // find corresponding Client from Db
            Client clientFromDb = null;

            try
            {
                clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();
            }
            catch(InvalidOperationException e)
            {
                Console.WriteLine("No clients have a ClientId that matches the Client passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }
            
            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if(updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.City = null;
                newAddress.USStateId = clientAddress.USStateId;
                newAddress.Zipcode = clientAddress.Zipcode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;
            
            // submit changes
            db.SubmitChanges();
        }
        
        internal static void AddUsernameAndPassword(Employee employee)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return employeeFromDb;
            }
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName != null;
        }


        //// TODO Items: ////
        
        // TODO: Allow any of the CRUD operations to occur here
        internal static void RunEmployeeQueries(Employee employee, string crudOperation)
        {
            crudOperation.ToLower();
            switch (crudOperation)
            {
                case "create":
                    db.Employees.InsertOnSubmit(employee);
                    break;
                case "read":
                    Employee employeeDB = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();
                    UserInterface.DisplayEmployeeInfo(employeeDB);
                    break;
                case "update":
                    db.Employees.InsertOnSubmit(employee);
                    break;
                case "delete":
                    db.Employees.DeleteOnSubmit(employee);
                    break;
                default:
                    break;
            }
            db.SubmitChanges();
        }

        // TODO: Animal CRUD Operations
        internal static void AddAnimal(Animal animal)
        {
            Animal animalFromDb = db.Animals.Where(a => a.AnimalId == animal.AnimalId).FirstOrDefault();
            animalFromDb.Category = animal.Category;
            animalFromDb.Demeanor = animal.Demeanor;
            animalFromDb.DietPlan = animal.DietPlan;
            animalFromDb.Gender = animal.Gender;
            animalFromDb.Age = animal.Age;
            animalFromDb.Employee = animal.Employee;
            animalFromDb.Name = animal.Name;
            animalFromDb.KidFriendly = animal.KidFriendly;
            animalFromDb.PetFriendly = animal.PetFriendly;
            animalFromDb.Weight = animal.Weight;
            db.SubmitChanges();
        }

        internal static Animal GetAnimalByID(int id)
        {
            Animal animal = db.Animals.Where(m => m.AnimalId == id).FirstOrDefault();
            return animal;
        }

        internal static void UpdateAnimal(int animalId, Dictionary<int, string> updates)
        {
            var acquiredAnimal = db.Animals.Where(a => a.AnimalId == animalId).FirstOrDefault();
            foreach (KeyValuePair<int, string> item in updates)
            {
                switch (item.Key)
                {
                    case 1:
                        bool isItThere = db.Categories.Any(c => c.CategoryId == int.Parse(item.Value));

                        if (isItThere == true)
                        {
                            acquiredAnimal.CategoryId = int.Parse(item.Value);
                        }
                        break;
                    case 2:
                        acquiredAnimal.Name = item.Value;
                        break;
                    case 3:
                        acquiredAnimal.Age = int.Parse(item.Value);
                        break;
                    case 4:
                        acquiredAnimal.Demeanor = item.Value;
                        break;
                    case 5:
                        acquiredAnimal.KidFriendly = Convert.ToBoolean(item.Value);
                        break;
                    case 6:
                        acquiredAnimal.PetFriendly = Convert.ToBoolean(item.Value);
                        break;
                    case 7:
                        acquiredAnimal.Weight = int.Parse(item.Value);
                        break;
                }
            }
            db.SubmitChanges();
        }

        internal static void RemoveAnimal(Animal animal)
        {
            db.Animals.DeleteOnSubmit(animal);
            db.SubmitChanges();
        }
        
        // TODO: Animal Multi-Trait Search
        internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates) // parameter(s)?
        {
            IQueryable<Animal> searchedAnimal = db.Animals.Where(a => a.Category.CategoryId == int.Parse(updates[1]) && a.Name == updates[2] && a.Age == int.Parse(updates[3]) && a.Demeanor == updates[4] && a.KidFriendly == Convert.ToBoolean(updates[5]) && a.PetFriendly == Convert.ToBoolean(updates[6]) && a.Weight == int.Parse(updates[7]));
            return searchedAnimal;
        }
         
        // TODO: Misc Animal Things
        internal static int GetCategoryId(string categoryName)
        {
            var acquiredCategoryIdObject = db.Categories.Where(c => c.Name == categoryName).FirstOrDefault();
            int acquiredCategoryId = acquiredCategoryIdObject.CategoryId;
            return acquiredCategoryId;
        }
    
        
        internal static Room GetRoom(int animalId)
        {
            var dbRoom = db.Rooms.Where(s => s.AnimalId == animalId).FirstOrDefault();
            return dbRoom;
        }
        
        internal static int GetDietPlanId(string dietPlanName)
        {
        var acquiredDietPlan = db.DietPlans.Where(d => d.DietPlanId.ToString() == dietPlanName).FirstOrDefault();
        int acquiredDietPlanInt = acquiredDietPlan.DietPlanId;
        return acquiredDietPlanInt;
    }

        // TODO: Adoption CRUD Operations
        internal static void Adopt(Animal animal, Client client)
        {
            animal.AdoptionStatus = "Pending";
            Adoption adoption = new Adoption();
            adoption.AnimalId = animal.AnimalId;
            adoption.ClientId = client.ClientId;
            db.Adoptions.InsertOnSubmit(adoption);
            db.SubmitChanges();
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {
            var pendingAdoptions = db.Adoptions.Where(a => a.ApprovalStatus == "Pending");
            return pendingAdoptions;
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            if(isAdopted)
            {
                adoption.Animal.AdoptionStatus = "Adopted";
                adoption.ApprovalStatus = "Approved";
                adoption.PaymentCollected = true;
            }
            else
            {
                adoption.Animal.AdoptionStatus = "Open";
                adoption.ApprovalStatus = "Denied";
                adoption.PaymentCollected = false;
            }
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            var adoption = db.Adoptions.Where(a => a.AnimalId == animalId && a.ClientId == clientId).FirstOrDefault();
            db.Adoptions.DeleteOnSubmit(adoption);
            db.SubmitChanges();
        }

        // TODO: Shots Stuff
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
             var animalShots = db.AnimalShots.Where(s => s.Animal == animal);
            return animalShots;
        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            var shot = db.Shots.Where(s => s.Name == shotName);
            var animalShot = db.AnimalShots.Where(a => a.AnimalId == animal.AnimalId).Where(b => b.ShotId == Convert.ToInt32(shot)).FirstOrDefault();
            animalShot.DateReceived = new DateTime(2020,3,11);
            db.AnimalShots.InsertOnSubmit(animalShot);
            db.SubmitChanges();
        }
    }
}