﻿using Echovoice.JSON;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchContactsAzureSearch
{
    class Program
    {
        #region Variables
        private static string searchServiceName = "";
        private static string searchServiceApiKey = "";
        private static string azureSearchIndex = "";
        private static SearchServiceClient _searchServiceClient;
        private static SearchIndexClient _searchIndexClient;
        #endregion

        #region Main
        static void Main(string[] args)
        {
            // Create an HTTP reference to the catalog index.
            _searchServiceClient = new SearchServiceClient(searchServiceName, new SearchCredentials(searchServiceApiKey));
            _searchIndexClient = (SearchIndexClient)_searchServiceClient.Indexes.GetClient(azureSearchIndex);

            Console.WriteLine("{0}, Deleting index...\n", azureSearchIndex);
            DeleteIndex();

            Console.WriteLine("{0}, Creating Index...\n", azureSearchIndex);
            if (CreateIndex() == false)
            {
                Console.ReadLine();
                return;
            }
            Console.WriteLine("{0}, Uploading content...\n", azureSearchIndex);
            UploadContent();

            // UploadContacts();
            Console.WriteLine("\nPress any key to continue\n");
            Console.ReadLine();
        }
        #endregion

        #region Methods
        private static bool DeleteIndex()
        {
            // Delete the Index
            try
            {
                _searchServiceClient.Indexes.Delete(azureSearchIndex);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting index: {0}\r\n", ex.Message);
                Console.WriteLine("Check that the searchServiceName and searchServiceApiKey are correct!");
                return false;
            }
            return true;
        }

        private static bool CreateIndex()
        {
            // Create an Azure Search index based on the schema.
            // Enable all CORS origins
            // Please do not enable CORS origins everwhere like
            // the code below this is for testing purposes only!
            CorsOptions cors = new CorsOptions();
            List<string> origins = new List<string>
            {
                "*"
            };
            cors.AllowedOrigins = origins;
            cors.MaxAgeInSeconds = 300;
            try
            {
                var definition = new Index
                {
                    Name = azureSearchIndex,
                    CorsOptions = cors,
                    // Creating all the fields in the index
                    // IsKey: Specifies the Primary key for the index. It must be unique and represented as a String.
                    // Example: "id", DataType.String is fine | "id", DatypeType.Int32 is not
                    // IsFacetable: 
                    // IsFilterable: 
                    // IsRetrievable: 
                    // IsSearchable: Specifies whether a field should be searchable or not
                    // IsSortable: Can we sort the field
                    Fields = new[]
                    {
                        #region Fields
                        new Field("id", DataType.Int32) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = false, IsSortable = true },
                        new Field("versionValue", DataType.Int32) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = false, IsSortable = true },
                        new Field("uuid", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("createdBy", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("createdDate", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("lastModifiedBy", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("lastModifiedDate", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("companyId", DataType.Int32) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = false, IsSortable = true },
                        new Field("name", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("displayName", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("salutation", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("firstName", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("middleName", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("lastName", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("nickName", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("suffix", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("genderId", DataType.Int32) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = false, IsSortable = true },
                        new Field("businessUnitId", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("dateOfBirth", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },

                        // contactsTypes Array [1] 
                        new Field("contactTypes", DataType.Collection(DataType.String)) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = false },

                        new Field("accountingReference", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("referenceId", DataType.String) { IsKey = true, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },

                        // languageMappings Array [2] 
                        new Field("languageMappings", DataType.Collection(DataType.String)) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = false },

                        new Field("rating", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("primaryNumber", DataType.Collection(DataType.String)) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = false },

                        // Numbers Array [3] 
                        new Field("numbers", DataType.Collection(DataType.String)) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = false },
                        new Field("primaryAddress", DataType.Collection(DataType.String)) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = false },

                        // Testing GeographyPoint for 'lat' and 'lng' 
                        new Field("lat", DataType.GeographyPoint) { IsKey = false, IsFacetable = false, IsFilterable = true, IsRetrievable = true, IsSearchable = false, IsSortable = true },
                        new Field("lng", DataType.GeographyPoint) { IsKey = false, IsFacetable = false, IsFilterable = true, IsRetrievable = true, IsSearchable = false, IsSortable = true },

                        // Addresses Array [4] 
                        new Field("addresses", DataType.Collection(DataType.String)) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = false },
                        new Field("primaryEmail", DataType.Collection(DataType.String)) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = false },

                        // Emails Array [5]
                        new Field("emails", DataType.Collection(DataType.String)) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = false },

                        // Qualifications Array [6]
                        new Field("qualifications", DataType.Collection(DataType.String)) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = false },

                        // Eligibilties Array [7]
                        new Field("eligibilities", DataType.Collection(DataType.String)) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = false },

                        // CriteriaHierarchy Array [8]
                        new Field("criteriaHierarchy", DataType.Collection(DataType.String)) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = false },

                        new Field("hasTransportation", DataType.Boolean) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = false, IsSortable = true },
                        new Field("hasChildren", DataType.Boolean) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = false, IsSortable = true },
                        new Field("notes", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("companyName", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("website", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("region", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("countryOfOrigin", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("countryOfResidence", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("countryOfNationality", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("active", DataType.Boolean) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = false, IsSortable = true },
                        new Field("activeNote", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("availability", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("experience", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("registeredTaxId", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("bankAccount", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("sortCode", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("iban", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("swift", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("eftId", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("eftName", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("paymentMethodId", DataType.Int32) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = false, IsSortable = true },
                        new Field("paymentMethodName", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("paymentAccount", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("registeredTax", DataType.Boolean) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = false, IsSortable = true },
                        new Field("registeredTaxIdDescription", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("employmentCategoryId", DataType.Int32) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = false, IsSortable = true },
                        new Field("assignmentTierId", DataType.Int32) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = false, IsSortable = true },
                        new Field("timeZone", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("ethnicity", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },

                        // Document Object
                        new Field("document", DataType.Collection(DataType.String)) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = false },

                        new Field("imagePath", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("outOfOffice", DataType.Boolean) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = false, IsSortable = true },
                        new Field("disableUpcomingReminder", DataType.Boolean) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = false, IsSortable = true },
                        new Field("disableCloseReminder", DataType.Boolean) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = false, IsSortable = true },
                        new Field("disableConfirmReminder", DataType.Boolean) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = false, IsSortable = true },
                        new Field("bankAccountDescription", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("timeWorked", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("activationDate", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("originalStartDate", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("datePhotoSentToPrinter", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("datePhotoSentToInterpreter", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("inductionDate", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("reActivationDate", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("iolNrcpdNumber", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("referralSource", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("refereeSourceName", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("recruiterName", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("taleoId", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("bankAccountReference", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },

                        // Status Object
                        new Field("status", DataType.Collection(DataType.String)) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = false },

                        new Field("disableConfirmationEmails", DataType.Boolean) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = false, IsSortable = true },
                        new Field("disableOfferEmails", DataType.Boolean) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = false, IsSortable = true },
                        new Field("disableAutoOffers", DataType.Boolean) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = false, IsSortable = true },
                        new Field("currencyCodeId", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("currencySymbol", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },
                        new Field("bankBranch", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true },

                        // Services Array [9]
                        new Field("services", DataType.Collection(DataType.String)) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = false },
                        new Field("enableAllServices", DataType.Boolean) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = false, IsSortable = true },
                        new Field("isSynchronized", DataType.Boolean) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = false, IsSortable = true },
                        new Field("lastSynchronizedDate", DataType.String) { IsKey = false, IsFacetable = true, IsFilterable = true, IsRetrievable = true, IsSearchable = true, IsSortable = true }
                        #endregion
                    }
                };
                // Create the index definition
                _searchServiceClient.Indexes.Create(definition);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating index: {0}\r\n", ex.Message);
                return false;
            }
            return true;
        }

        private static void UploadContent()
        {
            #region First Index Batch
            // This will open the JSON file, parse it and upload the documents in a batch
            List<IndexAction> indexOperations = new List<IndexAction>();
            JArray contact = JArray.Parse(File.ReadAllText(@"interpreterintelligencecontacts4.json"));

            foreach (var array in contact)
            {
                //using (StreamReader jsonfile = File.OpenText(file))
                //{
                #region Deserialize JSON
                //totalCounter++;
                Document doc = new Document
                {
                    //string json = jsonfile.ReadToEnd();

                    //dynamic array = JsonConvert.DeserializeObject(json);

                    // You can write the code like this doc.Add("uuid", array["uuid"]); if you know that the field 'uuid' in your
                    // json will not have a null value.  If the json does have a null value for 'uuid' you must write the code like this:
                    // doc.Add("uuid", array["uuid"] == null ? "" : array["uuid"]);.  This will add "" to all the documents where uuid
                    // is null.
                    { "id", array["id"] == null ? -1 : array["id"] },
                    { "versionValue", array["versionValue"] == null ? -1 : array["versionValue"] },
                    { "uuid", array["uuid"] == null ? "" : array["uuid"] },
                    { "createdBy", array["createdBy"] == null ? "" : array["createdBy"] },
                    { "createdDate", array["createdDate"] == null ? "" : array["createdDate"] },
                    { "lastModifiedBy", array["lastModifiedBy"] == null ? "" : array["lastModifiedBy"] },
                    { "lastModifiedDate", array["lastModifiedDate"] == null ? "" : array["lastModifiedDate"] },
                    { "companyId", array["companyId"] == null ? -1 : array["companyId"] },
                    { "name", array["name"] == null ? "" : array["name"] },
                    { "displayName", array["displayName"] == null ? "" : array["displayName"] },
                    { "salutation", array["salutation"] == null ? "" : array["salutation"] },
                    { "firstName", array["firstName"] == null ? "" : array["firstName"] },
                    { "middleName", array["middleName"] == null ? "" : array["middleName"] },
                    { "lastName", array["lastName"] == null ? "" : array["lastName"] },
                    { "nickName", array["nickName"] == null ? "" : array["nickName"] },
                    { "suffix", array["suffix"] == null ? "" : array["suffix"] },
                    { "genderId", array["genderId"] == null ? -1 : array["genderId"] },
                    { "businessUnitId", array["businessUnitId"] == null ? "" : array["businessUnitId"] },
                    { "dateOfBirth", array["dateOfBirth"] == null ? "" : array["dateOfBirth"] }
                };

                // You can Use JArray/JObject when parsing the JSON.
                // You must cast array["contactTypes"] to type JArray.
                // I am using JToken as it removes the need to cast.
                // ContactTypes Array [1]
                if (array["contactTypes"] != null)
                {
                    JToken contactTypesArray = array["contactTypes"];
                    List<string> contactTypesList = new List<string>();
                    if (contactTypesArray != null)
                    {
                        foreach (var item in contactTypesArray)
                        {
                            contactTypesList.Add(item.ToString());
                        }
                    }
                    doc.Add("contactTypes", contactTypesList);
                }

                doc.Add("accountingReference", array["accountingReference"] == null ? "" : array["accountingReference"]);
                doc.Add("referenceId", array["referenceId"] == null ? "" : array["referenceId"]);

                // LanguageMappings Array [2]
                if (array["languageMappings"] != null)
                {
                    JToken languageMappingsArray = array["languageMappings"];
                    List<string> languageMappingsList = new List<string>();
                    if (languageMappingsArray != null)
                    {
                        foreach (var item in languageMappingsArray)
                        {
                            languageMappingsList.Add(item.ToString());
                        }
                    }
                    doc.Add("languageMappings", languageMappingsList);
                }

                doc.Add("rating", array["rating"] == null ? "" : array["rating"]);

                // Object Primary Number
                if (array["primaryNumber"] != null)
                {
                    JToken primaryNumberObject = array["primaryNumber"];
                    List<string> primaryNumberList = new List<string>();
                    if (primaryNumberObject != null)
                    {
                        foreach (var item in primaryNumberObject)
                        {
                            primaryNumberList.Add(item.ToString());
                        }
                    }
                    doc.Add("primaryNumber", primaryNumberList);
                }

                //doc.Add("primaryNumber", array["primaryNumber"] == null ? "" : array["primaryNumber"]);

                // Numbers Array [3]
                if (array["numbers"] != null)
                {
                    JToken numbersArray = array["numbers"];
                    List<string> numbersList = new List<string>();
                    if (numbersArray != null)
                    {
                        foreach (var item in numbersArray)
                        {
                            numbersList.Add(item.ToString());
                        }
                    }
                    doc.Add("numbers", numbersList);
                }
                // Object Primary Address
                if (array["primaryAddress"] != null)
                {
                    JToken primaryAddressObject = array["primaryAddress"];
                    List<string> primaryAddressList = new List<string>();
                    if (primaryAddressObject != null)
                    {
                        foreach (var item in primaryAddressObject)
                        {
                            primaryAddressList.Add(item.ToString());
                        }
                    }
                    doc.Add("primaryAddress", primaryAddressList);
                }

                //doc.Add("primaryAddress", array["primaryAddress"]);

                // This is not working now, when I try to index lat/lng
                // I get not a valid Spatial value.  I am not sure why!
                // I was hoping to implement a map on the web page to 
                // display the location of the contacts.
                // Testing GeographyPoint for 'lat' and 'lng' 
                //doc.Add("lat", array["lat"] == null ? 41.888 : array["lat"]);
                //doc.Add("lng", array["lng"] == null ? -8.454 : array["lng"]);

                // Addresses Array [4]
                if (array["addresses"] != null)
                {
                    JToken addressesArray = array["addresses"];
                    List<string> addressesList = new List<string>();
                    if (addressesArray != null)
                    {
                        foreach (var item in addressesArray)
                        {
                            addressesList.Add(item.ToString());
                        }
                    }
                    doc.Add("addresses", addressesList);
                }

                // Object Primary Email
                if (array["primaryEmail"] != null)
                {
                    JToken primaryEmailObject = array["primaryEmail"];
                    List<string> primaryEmailList = new List<string>();
                    if (primaryEmailObject != null)
                    {
                        foreach (var item in primaryEmailObject)
                        {
                            primaryEmailList.Add(item.ToString());
                        }
                    }
                    doc.Add("primaryEmail", primaryEmailList);
                }
                //doc.Add("primaryEmail", array["primaryEmail"]);

                // Emails Array [5]
                if (array["emails"] != null)
                {
                    JToken emailsArray = array["emails"];
                    List<string> emailsList = new List<string>();
                    if (emailsArray != null)
                    {
                        foreach (var item in emailsArray)
                        {
                            emailsList.Add(item.ToString());
                        }
                    }
                    doc.Add("emails", emailsList);
                }

                // Qualifications Array [6]
                if (array["qualifications"] != null)
                {
                    JToken qualificationsArray = array["qualifications"];
                    List<string> qualificationsList = new List<string>();
                    if (qualificationsArray != null)
                    {
                        foreach (var item in qualificationsArray)
                        {
                            qualificationsList.Add(item.ToString());
                        }
                    }
                    doc.Add("qualifications", qualificationsList);
                }

                // Eligibilities Array [7]
                if (array["eligibilities"] != null)
                {
                    JToken eligibilitesArray = array["eligibilities"];
                    List<string> eligibilitiesList = new List<string>();
                    if (eligibilitesArray != null)
                    {
                        foreach (var item in eligibilitesArray)
                        {
                            eligibilitiesList.Add(item.ToString());
                        }
                    }
                    doc.Add("eligibilities", eligibilitiesList);
                }

                // CriteriaHierarchy Array [8]
                if (array["criteriaHierarchy"] != null)
                {
                    JToken criteriaHierarchyArray = array["criteriaHierarchy"];
                    List<dynamic> criteriaHierarchyList = new List<dynamic>();
                    if (criteriaHierarchyArray != null)
                    {
                        foreach (var item in criteriaHierarchyArray)
                        {
                            criteriaHierarchyList.Add(item);
                        }
                    }
                    doc.Add("criteriaHierarchy", criteriaHierarchyList);
                }

                doc.Add("hasTransportation", array["hasTransportation"] == null ? false : array["hasTransportation"]);
                doc.Add("hasChildren", array["hasChildren"] == null ? false : array["hasChildren"]);
                doc.Add("notes", array["notes"] == null ? "" : array["notes"]);
                doc.Add("companyName", array["comanyName"] == null ? "" : array["companyName"]);
                doc.Add("website", array["website"] == null ? "" : array["website"]);
                doc.Add("region", array["region"] == null ? "" : array["region"]);
                doc.Add("countryOfOrigin", array["countryOfOrigin"] == null ? "" : array["countryOfOrigin"]);
                doc.Add("countryOfResidence", array["countryOfResidence"] == null ? "" : array["countryOfResidence"]);
                doc.Add("countryOfNationality", array["countryOfNationality"] == null ? "" : array["countryOfNationality"]);
                doc.Add("active", array["active"] == null ? false : array["active"]);
                doc.Add("activeNote", array["activeNote"] == null ? "" : array["activeNote"]);
                doc.Add("availability", array["availability"] == null ? "" : array["availability"]);
                doc.Add("experience", array["experience"] == null ? "" : array["experience"]);
                doc.Add("registeredTaxId", array["registeredTaxId"] == null ? "" : array["registeredTaxId"]);
                doc.Add("bankAccount", array["bankAccount"] == null ? "" : array["bankAccount"]);
                doc.Add("sortCode", array["sortCode"] == null ? "" : array["sortCode"]);
                doc.Add("iban", array["iban"] == null ? "" : array["iban"]);
                doc.Add("swift", array["swift"] == null ? "" : array["swift"]);
                doc.Add("eftId", array["eftId"] == null ? "" : array["eftId"]);
                doc.Add("eftName", array["eftName"] == null ? "" : array["eftName"]);
                doc.Add("paymentMethodId", array["paymentMethodId"] == null ? -1 : array["paymentMethodId"]);
                doc.Add("paymentMethodName", array["paymentMethodName"] == null ? "" : array["paymentMethodName"]);
                doc.Add("paymentAccount", array["paymentAccount"] == null ? "" : array["paymentAccount"]);
                doc.Add("registeredTax", array["registeredTax"] == null ? false : array["registeredTax"]);
                doc.Add("registeredTaxIdDescription", array["registeredTaxIdDescription"] == null ? "" : array["registeredTaxIdDescription"]);
                doc.Add("employmentCategoryId", array["employmentCategoryId"] == null ? -1 : array["employmentCategoryId"]);
                doc.Add("assignmentTierId", array["assignmentTierId"] == null ? -1 : array["assignmentTierId"]);
                doc.Add("timeZone", array["timeZone"] == null ? "" : array["timeZone"]);
                doc.Add("ethnicity", array["ethnicity"] == null ? "" : array["ethnicity"]);

                // Object Document
                if (array["document"] != null)
                {
                    JToken documentObject = array["document"];
                    List<string> documentList = new List<string>();
                    if (documentObject != null)
                    {
                        foreach (var item in documentObject)
                        {
                            documentList.Add(item.ToString());
                        }
                    }
                    doc.Add("document", documentList);
                }

                //doc.Add("document", array["document"]);

                doc.Add("imagePath", array["imagePath"] == null ? "" : array["imagePath"]);
                doc.Add("outOfOffice", array["outOfOffice"] == null ? false : array["outOfOffice"]);
                doc.Add("disableUpcomingReminder", array["disableUpcomingReminder"] == null ? false : array["disableUpcomingReminder"]);
                doc.Add("disableCloseReminder", array["disableCloseReminder"] == null ? false : array["disableCloseReminder"]);
                doc.Add("disableConfirmReminder", array["disableConfirmReminder"] == null ? false : array["disableConfirmReminder"]);
                doc.Add("bankAccountDescription", array["bankAccountDescription"] == null ? "" : array["bankAccountDescription"]);
                doc.Add("timeWorked", array["timeWorked"] == null ? "" : array["timeWorked"]);
                doc.Add("activationDate", array["activationDate"] == null ? "" : array["activationDate"]);
                doc.Add("originalStartDate", array["originalStartDate"] == null ? "" : array["originalStartDate"]);
                doc.Add("datePhotoSentToPrinter", array["datePhotoSentToPrinter"] == null ? "" : array["datePhotoSentToPrinter"]);
                doc.Add("datePhotoSentToInterpreter", array["datePhotoSentTpInterpreter"] == null ? "" : array["datePhotoSentToInterpreter"]);
                doc.Add("inductionDate", array["inductionDate"] == null ? "" : array["inductionDate"]);
                doc.Add("reActivationDate", array["reActivationDate"] == null ? "" : array["reActivationDate"]);
                doc.Add("iolNrcpdNumber", array["iolNrcpdNumber"] == null ? "" : array["iolNrcpdNumber"]);
                doc.Add("referralSource", array["referralSource"] == null ? "" : array["referralSource"]);
                doc.Add("refereeSourceName", array["refereeSourceName"] == null ? "" : array["refereeSourceName"]);
                doc.Add("recruiterName", array["recruiterName"] == null ? "" : array["recruiterName"]);
                doc.Add("taleoId", array["taleoId"] == null ? "" : array["taleoId"]);
                doc.Add("bankAccountReference", array["bankAccountreference"] == null ? "" : array["bankAccountReference"]);

                // Object Status
                if (array["status"] != null)
                {
                    JToken statusObject = array["status"];
                    List<string> statusList = new List<string>();
                    if (statusObject != null)
                    {
                        foreach (var item in statusObject)
                        {
                            statusList.Add(item.ToString());
                        }
                    }
                    doc.Add("status", statusList);
                }

                //doc.Add("status", array["status"]);

                doc.Add("disableConfirmationEmails", array["disableConfirmationEmails"] == null ? false : array["disableConfirmationEmails"]);
                doc.Add("disableOfferEmails", array["disableOfferEmails"] == null ? false : array["disableOfferEmails"]);
                doc.Add("disableAutoOffers", array["disableAutoOffers"] == null ? false : array["disableAutoOffers"]);
                doc.Add("currencyCodeId", array["currencyCodeId"] == null ? "" : array["currencyCodeId"]);
                doc.Add("currencySymbol", array["currencySymbol"] == null ? "" : array["currencySymbol"]);
                doc.Add("bankBranch", array["bankBranch"] == null ? "" : array["bankBranch"]);

                // Services Array [9]
                if (array["services"] != null)
                {
                    JToken servicesArray = array["services"];
                    List<string> servicesList = new List<string>();
                    if (servicesArray != null)
                    {
                        foreach (var item in servicesArray)
                        {
                            servicesList.Add(item.ToString());
                        }
                    }
                    doc.Add("services", servicesList);
                }

                doc.Add("enableAllServices", array["enableAllServices"] == null ? false : array["enableAllServices"]);
                doc.Add("isSynchronized", array["isSynchronized"] == null ? false : array["isSynchronized"]);
                doc.Add("lastSynchronizedDate", array["lastSynchronizedDate"] == null ? "" : array["lastSynchronizedDate"]);
                #endregion

                // Upload the documents to Azure Search
                indexOperations.Add(IndexAction.Upload(doc));

                try
                {
                    _searchIndexClient.Documents.Index(new IndexBatch(indexOperations));
                    Console.WriteLine("Documents" + doc);
                    Console.WriteLine("Index operations: " + indexOperations);
                    //Console.WriteLine(contact);
                }
                catch (IndexBatchException e)
                {
                    // Sometimes when your Search service is under load, indexing will fail for some of the documents in
                    // the batch. Depending on your application, you can take compensating actions like delaying and
                    // retrying. For this simple demo, we just log the failed document keys and continue.
                    Console.WriteLine(
                    "Failed to index some of the documents: {0}",
                           String.Join(", ", e.IndexingResults.Where(r => !r.Succeeded).Select(r => r.Key)));
                }
            }
            #endregion
        }
    }
}
#region Testing
//        private static void UploadContent()
//        {
//            // Scan the JSON files from Interpreter Intelligence contacts and upload them to Azure Search.
//            var indexOperations = new List<IndexAction>();

//            string[] files = Directory.GetFiles(@"C:\Users\Ronan\source\repos\SearchContactsAzureSearch\SearchContactsAzureSearch\json", "*.json", SearchOption.TopDirectoryOnly);
//            int totalCounter = 0;

//            try
//            {
//                foreach (var file in files)
//                {
//                    using (StreamReader jsonfile = File.OpenText(file))
//                    {
//                        #region Deserialize JSON
//                        totalCounter++;
//                        Document doc = new Document();
//                        string json = jsonfile.ReadToEnd();

//                        dynamic array = JsonConvert.DeserializeObject(json);

//                        // You can write the code like this doc.Add("uuid", array["uuid"].Value); if you know that the field 'uuid' in your
//                        // json will not have a null value.  If the json does have a null value for 'uuid' you must write the code like this:
//                        // doc.Add("uuid", array["uuid"] == null ? "" : array["uuid"].Value);.  This will add "" to all the documents where uuid
//                        // is null.
//                        doc.Add("id", array["id"] == null ? -1 : array["id"].Value);
//                        doc.Add("versionValue", array["versionValue"] == null ? -1 : array["versionValue"].Value);
//                        doc.Add("uuid", array["uuid"] == null ? "" : array["uuid"].Value);
//                        doc.Add("createdBy", array["createdBy"] == null ? "" : array["createdBy"].Value);
//                        doc.Add("createdDate", array["createdDate"] == null ? "" : array["createdDate"].Value);
//                        doc.Add("lastModifiedBy", array["lastModifiedBy"] == null ? "" : array["lastModifiedBy"].Value);
//                        doc.Add("lastModifiedDate", array["lastModifiedDate"] == null ? "" : array["lastModifiedDate"].Value);
//                        doc.Add("companyId", array["companyId"] == null ? -1 : array["companyId"].Value);
//                        doc.Add("name", array["name"] == null ? "" : array["name"].Value);
//                        doc.Add("displayName", array["displayName"] == null ? "" : array["displayName"].Value);
//                        doc.Add("salutation", array["salutation"] == null ? "" : array["salutation"].Value);
//                        doc.Add("firstName", array["firstName"] == null ? "" : array["firstName"].Value);
//                        doc.Add("middleName", array["middleName"] == null ? "" : array["middleName"].Value);
//                        doc.Add("lastName", array["lastName"] == null ? "" : array["lastName"].Value);
//                        doc.Add("nickName", array["nickName"] == null ? "" : array["nickName"].Value);
//                        doc.Add("suffix", array["suffix"] == null ? "" : array["suffix"].Value);
//                        doc.Add("genderId", array["genderId"] == null ? -1 : array["genderId"].Value);
//                        doc.Add("businessUnitId", array["businessUnitId"] == null ? "" : array["businessUnitId"].Value);
//                        doc.Add("dateOfBirth", array["dateOfBirth"] == null ? "" : array["dateOfBirth"].Value);

//                        // ContactTypes Array [1]
//                        if (array["contactTypes"] != null)
//                        {
//                            JArray contactTypesArray = array["contactTypes"];
//                            List<string> contactTypesList = new List<string>();
//                            if (contactTypesArray != null)
//                            {
//                                foreach (var item in contactTypesArray)
//                                {
//                                    contactTypesList.Add(item.ToString());
//                                }
//                            }

//                            doc.Add("contactTypes", contactTypesList);
//                        }

//                        doc.Add("accountingReference", array["accountingReference"] == null ? "" : array["accountingReference"].Value);
//                        doc.Add("referenceId", array["referenceId"] == null ? "" : array["referenceId"].Value);

//                        // LanguageMappings Array [2]
//                        if (array["languageMappings"] != null)
//                        {
//                            JArray languageMappingsArray = array["languageMappings"];
//                            List<string> languageMappingsList = new List<string>();
//                            if (languageMappingsArray != null)
//                            {
//                                foreach (var item in languageMappingsArray)
//                                {
//                                    languageMappingsList.Add(item.ToString());
//                                }
//                            }

//                            doc.Add("languageMappings", languageMappingsList);
//                        }

//                        doc.Add("rating", array["rating"] == null ? "" : array["rating"].Value);

//                        // Object
//                        if (array["primaryNumber"] != null)
//                        {
//                            JObject primaryNumberObject = array["primaryNumber"];
//                            List<string> primaryNumberList = new List<string>();
//                            if (primaryNumberObject != null)
//                            {
//                                foreach (var item in primaryNumberObject)
//                                {
//                                    primaryNumberList.Add(item.ToString());
//                                }
//                            }

//                            doc.Add("primaryNumber", primaryNumberList);
//                        }

//                        //doc.Add("primaryNumber", array["primaryNumber"] == null ? "" : array["primaryNumber"].Value);

//                        // Numbers Array [3]
//                        if (array["numbers"] != null)
//                        {
//                            JArray numbersArray = array["numbers"];
//                            List<string> numbersList = new List<string>();
//                            if (numbersArray != null)
//                            {
//                                foreach (var item in numbersArray)
//                                {
//                                    numbersList.Add(item.ToString());
//                                }
//                            }

//                            doc.Add("numbers", numbersList);
//                        }
//                        // Object
//                        if (array["primaryAddress"] != null)
//                        {
//                            JObject primaryAddressObject = array["primaryAddress"];
//                            List<string> primaryAddressList = new List<string>();
//                            if (primaryAddressObject != null)
//                            {
//                                foreach (var item in primaryAddressObject)
//                                {
//                                    primaryAddressList.Add(item.ToString());
//                                }
//                            }

//                            doc.Add("primaryAddress", primaryAddressList);
//                        }

//                        //doc.Add("primaryAddress", array["primaryAddress"]);

//                        // Testing GeographyPoint for 'lat' and 'lng' 
//                        // doc.Add("lat", array["lat"].Value);
//                        // doc.Add("lng", array["lng"].Value);

//                        // Addresses Array [4]
//                        if (array["addresses"] != null)
//                        {
//                            JArray addressesArray = array["addresses"];
//                            List<string> addressesList = new List<string>();
//                            if (addressesArray != null)
//                            {
//                                foreach (var item in addressesArray)
//                                {
//                                    addressesList.Add(item.ToString());
//                                }
//                            }

//                            doc.Add("addresses", addressesList);
//                        }

//                        // Object
//                        if (array["primaryEmail"] != null)
//                        {
//                            JObject primaryEmailObject = array["primaryEmail"];
//                            List<string> primaryEmailList = new List<string>();
//                            if (primaryEmailObject != null)
//                            {
//                                foreach (var item in primaryEmailObject)
//                                {
//                                    primaryEmailList.Add(item.ToString());
//                                }
//                            }

//                            doc.Add("primaryEmail", primaryEmailList);
//                        }
//                        //doc.Add("primaryEmail", array["primaryEmail"]);

//                        // Emails Array [5]
//                        if (array["emails"] != null)
//                        {
//                            JArray emailsArray = array["emails"];
//                            List<string> emailsList = new List<string>();
//                            if (emailsArray != null)
//                            {
//                                foreach (var item in emailsArray)
//                                {
//                                    emailsList.Add(item.ToString());
//                                }
//                            }

//                            doc.Add("emails", emailsList);
//                        }

//                        // Qualifications Array [6]
//                        if (array["qualifications"] != null)
//                        {
//                            JArray qualificationsArray = array["qualifications"];
//                            List<string> qualificationsList = new List<string>();
//                            if (qualificationsArray != null)
//                            {
//                                foreach (var item in qualificationsArray)
//                                {
//                                    qualificationsList.Add(item.ToString());
//                                }
//                            }

//                            doc.Add("qualifications", qualificationsList);
//                        }

//                        // Eligibilities Array [7]
//                        if (array["eligibilities"] != null)
//                        {
//                            JArray eligibilitesArray = array["eligibilities"];
//                            List<string> eligibilitiesList = new List<string>();
//                            if (eligibilitesArray != null)
//                            {
//                                foreach (var item in eligibilitesArray)
//                                {
//                                    eligibilitiesList.Add(item.ToString());
//                                }
//                            }

//                            doc.Add("eligibilities", eligibilitiesList);
//                        }

//                        // CriteriaHierarchy Array [8]
//                        if (array["criteriaHierarchy"] != null)
//                        {
//                            JArray criteriaHierarchyArray = array["criteriaHierarchy"];
//                            List<dynamic> criteriaHierarchyList = new List<dynamic>();
//                            if (criteriaHierarchyArray != null)
//                            {
//                                foreach (var item in criteriaHierarchyArray)
//                                {
//                                    criteriaHierarchyList.Add(item);
//                                }
//                            }

//                            doc.Add("criteriaHierarchy", criteriaHierarchyList);
//                        }

//                        doc.Add("hasTransportation", array["hasTransportation"] == null ? false : array["hasTransportation"].Value);
//                        doc.Add("hasChildren", array["hasChildren"] == null ? false : array["hasChildren"].Value);
//                        doc.Add("notes", array["notes"] == null ? "" : array["notes"].Value);
//                        doc.Add("companyName", array["comanyName"] == null ? "" : array["companyName"].Value);
//                        doc.Add("website", array["website"] == null ? "" : array["website"].Value);
//                        doc.Add("region", array["region"] == null ? "" : array["region"].Value);
//                        doc.Add("countryOfOrigin", array["countryOfOrigin"] == null ? "" : array["countryOfOrigin"].Value);
//                        doc.Add("countryOfResidence", array["countryOfResidence"] == null ? "" : array["countryOfResidence"].Value);
//                        doc.Add("countryOfNationality", array["countryOfNationality"] == null ? "" : array["countryOfNationality"].Value);
//                        doc.Add("active", array["active"] == null ? false : array["active"].Value);
//                        doc.Add("activeNote", array["activeNote"] == null ? "" : array["activeNote"].Value);
//                        doc.Add("availability", array["availability"] == null ? "" : array["availability"].Value);
//                        doc.Add("experience", array["experience"] == null ? "" : array["experience"].Value);
//                        doc.Add("registeredTaxId", array["registeredTaxId"] == null ? "" : array["registeredTaxId"].Value);
//                        doc.Add("bankAccount", array["bankAccount"] == null ? "" : array["bankAccount"].Value);
//                        doc.Add("sortCode", array["sortCode"] == null ? "" : array["sortCode"].Value);
//                        doc.Add("iban", array["iban"] == null ? "" : array["iban"].Value);
//                        doc.Add("swift", array["swift"] == null ? "" : array["swift"].Value);
//                        doc.Add("eftId", array["eftId"] == null ? "" : array["eftId"].Value);
//                        doc.Add("eftName", array["eftName"] == null ? "" : array["eftName"].Value);
//                        doc.Add("paymentMethodId", array["paymentMethodId"] == null ? -1 : array["paymentMethodId"].Value);
//                        doc.Add("paymentMethodName", array["paymentMethodName"] == null ? "" : array["paymentMethodName"].Value);
//                        doc.Add("paymentAccount", array["paymentAccount"] == null ? "" : array["paymentAccount"].Value);
//                        doc.Add("registeredTax", array["registeredTax"] == null ? false : array["registeredTax"].Value);
//                        doc.Add("registeredTaxIdDescription", array["registeredTaxIdDescription"] == null ? "" : array["registeredTaxIdDescription"].Value);
//                        doc.Add("employmentCategoryId", array["employmentCategoryId"] == null ? -1 : array["employmentCategoryId"].Value);
//                        doc.Add("assignmentTierId", array["assignmentTierId"] == null ? -1 : array["assignmentTierId"].Value);
//                        doc.Add("timeZone", array["timeZone"] == null ? "" : array["timeZone"].Value);
//                        doc.Add("ethnicity", array["ethnicity"] == null ? "" : array["ethnicity"].Value);
//                        // Object
//                        if (array["document"] != null)
//                        {
//                            JObject documentObject = array["document"];
//                            List<string> documentList = new List<string>();
//                            if (documentObject != null)
//                            {
//                                foreach (var item in documentObject)
//                                {
//                                    documentList.Add(item.ToString());
//                                }
//                            }

//                            doc.Add("document", documentList);
//                        }

//                        //doc.Add("document", array["document"]);

//                        doc.Add("imagePath", array["imagePath"] == null ? "" : array["imagePath"].Value);
//                        doc.Add("outOfOffice", array["outOfOffice"] == null ? false : array["outOfOffice"].Value);
//                        doc.Add("disableUpcomingReminder", array["disableUpcomingReminder"] == null ? false : array["disableUpcomingReminder"].Value);
//                        doc.Add("disableCloseReminder", array["disableCloseReminder"] == null ? false : array["disableCloseReminder"].Value);
//                        doc.Add("disableConfirmReminder", array["disableConfirmReminder"] == null ? false : array["disableConfirmReminder"].Value);
//                        doc.Add("bankAccountDescription", array["bankAccountDescription"] == null ? "" : array["bankAccountDescription"].Value);
//                        doc.Add("timeWorked", array["timeWorked"] == null ? "" : array["timeWorked"].Value);
//                        doc.Add("activationDate", array["activationDate"] == null ? "" : array["activationDate"].Value);
//                        doc.Add("originalStartDate", array["originalStartDate"] == null ? "" : array["originalStartDate"].Value);
//                        doc.Add("datePhotoSentToPrinter", array["datePhotoSentToPrinter"] == null ? "" : array["datePhotoSentToPrinter"].Value);
//                        doc.Add("datePhotoSentToInterpreter", array["datePhotoSentTpInterpreter"] == null ? "" : array["datePhotoSentToInterpreter"].Value);
//                        doc.Add("inductionDate", array["inductionDate"] == null ? "" : array["inductionDate"].Value);
//                        doc.Add("reActivationDate", array["reActivationDate"] == null ? "" : array["reActivationDate"].Value);
//                        doc.Add("iolNrcpdNumber", array["iolNrcpdNumber"] == null ? "" : array["iolNrcpdNumber"].Value);
//                        doc.Add("referralSource", array["referralSource"] == null ? "" : array["referralSource"].Value);
//                        doc.Add("refereeSourceName", array["refereeSourceName"] == null ? "" : array["refereeSourceName"].Value);
//                        doc.Add("recruiterName", array["recruiterName"] == null ? "" : array["recruiterName"].Value);
//                        doc.Add("taleoId", array["taleoId"] == null ? "" : array["taleoId"].Value);
//                        doc.Add("bankAccountReference", array["bankAccountreference"] == null ? "" : array["bankAccountReference"].Value);
//                        // Object
//                        if (array["status"] != null)
//                        {
//                            JObject statusObject = array["status"];
//                            List<string> statusList = new List<string>();
//                            if (statusObject != null)
//                            {
//                                foreach (var item in statusObject)
//                                {
//                                    statusList.Add(item.ToString());
//                                }
//                            }

//                            doc.Add("status", statusList);
//                        }

//                        //doc.Add("status", array["status"]);

//                        doc.Add("disableConfirmationEmails", array["disableConfirmationEmails"] == null ? false : array["disableConfirmationEmails"].Value);
//                        doc.Add("disableOfferEmails", array["disableOfferEmails"] == null ? false : array["disableOfferEmails"].Value);
//                        doc.Add("disableAutoOffers", array["disableAutoOffers"] == null ? false : array["disableAutoOffers"].Value);
//                        doc.Add("currencyCodeId", array["currencyCodeId"] == null ? "" : array["currencyCodeId"].Value);
//                        doc.Add("currencySymbol", array["currencySymbol"] == null ? "" : array["currencySymbol"].Value);
//                        doc.Add("bankBranch", array["bankBranch"] == null ? "" : array["bankBranch"].Value);

//                        // Services Array [9]
//                        if (array["services"] != null)
//                        {
//                            JArray servicesArray = array["services"];
//                            List<string> servicesList = new List<string>();
//                            if (servicesArray != null)
//                            {
//                                foreach (var item in servicesArray)
//                                {
//                                    servicesList.Add(item.ToString());
//                                }
//                            }

//                            doc.Add("services", servicesList);
//                        }

//                        doc.Add("enableAllServices", array["enableAllServices"] == null ? false : array["enableAllServices"].Value);
//                        doc.Add("isSynchronized", array["isSynchronized"] == null ? false : array["isSynchronized"].Value);
//                        doc.Add("lastSynchronizedDate", array["lastSynchronizedDate"] == null ? "" : array["lastSynchronizedDate"].Value);
//                        #endregion

//                        indexOperations.Add(IndexAction.Upload(doc));

//                        if (indexOperations.Count >= 500)
//                        {
//                            Console.WriteLine("Writing {0} documents of {1} total documents...", indexOperations.Count, totalCounter);
//                            _searchIndexClient.Documents.Index(new IndexBatch(indexOperations));
//                            indexOperations.Clear();
//                        }
//                    }
//                }

//                if (indexOperations.Count >= 0)
//                {
//                    Console.WriteLine("Writing {0} documents of {1} total documents...", indexOperations.Count, totalCounter);
//                    _searchIndexClient.Documents.Index(new IndexBatch(indexOperations));
//                }
//            }
//            catch (IndexBatchException ex)
//            {
//                // Sometimes when the search service is under load, indexing will fail for some of the documents in the batch.
//                Console.WriteLine("Failed to index some of the documents: {0}", String.Join(", ", ex.IndexingResults.Where(r => !r.Succeeded).Select(r => r.Key)));
//            }
//        }

//        #region Testing method UploadContacts
//        public static void UploadContacts()
//        {
//            // This will open the json file parse it and upload the documents in a batch
//            List<IndexAction> indexOperations = new List<IndexAction>();
//            JArray json = JArray.Parse(File.ReadAllText(@"interpreterintelligencecontacts1.json"));

//            foreach (var array in json)
//            {
//                // Parsing the JSON 
//                var doc = new Document();
//                doc.Add("id", array["id"]);
//                doc.Add("versionValue", array["versionValue"]);
//                doc.Add("uuid", array["uuid"]);
//                doc.Add("createdBy", array["createdBy"]);
//                doc.Add("createdDate", array["createdDate"]);
//                doc.Add("lastModifiedBy", array["lastModifiedBy"]);

//                indexOperations.Add(IndexAction.Upload(doc));
//            }

//            try
//            {
//                _searchIndexClient.Documents.Index(new IndexBatch(indexOperations));
//            }
//            catch (IndexBatchException e)
//            {
//                // Sometimes when your Search service is under load, indexing will fail for some of the documents in
//                // the batch. Depending on your application, you can take compensating actions like delaying and
//                // retrying. For this simple demo, we just log the failed document keys and continue.
//                Console.WriteLine(
//                "Failed to index some of the documents: {0}",
//                       String.Join(", ", e.IndexingResults.Where(r => !r.Succeeded).Select(r => r.Key)));
//            }
//        }
//#endregion
//#endregion
//    }
#endregion
//}
#endregion