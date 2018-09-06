using Echovoice.JSON;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Newtonsoft.Json;
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
        private static string AzureSearchIndex = "";
        private static SearchServiceClient _searchServiceClient;
        private static SearchIndexClient _searchIndexClient;
        #endregion

        #region Main
        static void Main(string[] args)
        {
            // Create an HTTP reference to the catalog index.
            _searchServiceClient = new SearchServiceClient(searchServiceName, new SearchCredentials(searchServiceApiKey));
            _searchIndexClient = (SearchIndexClient)_searchServiceClient.Indexes.GetClient(AzureSearchIndex);

            Console.WriteLine("{0}, Deleting index...\n", AzureSearchIndex);
            DeleteIndex();

            Console.WriteLine("{0}, Creating Index...\n", AzureSearchIndex);

            if (CreateIndex() == false)
            {
                Console.ReadLine();
                return;
            }

            Console.WriteLine("{0}, Uploading content...\n", AzureSearchIndex);
            UploadContent();

            Console.WriteLine("\nPress any key to continue\n");
            Console.ReadLine();
        }
        #endregion

        #region Methods
        private static bool DeleteIndex()
        {
            // Delete the index, datasource and indexer.
            try
            {
                _searchServiceClient.Indexes.Delete(AzureSearchIndex);
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
            CorsOptions cors = new CorsOptions();
            List<string> origins = new List<string>
            {
                "*"
            };
            cors.AllowedOrigins = origins;

            try
            {
                var definition = new Index
                {
                    Name = AzureSearchIndex,
                    CorsOptions = cors,
                    Fields = new[]
                    {
                        #region Fields
                        new Field("id", DataType.Int32) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("versionValue", DataType.Int32) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("uuid", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("createdBy", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("createdDate", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("lastModifiedBy", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("lastModifiedDate", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("company_id", DataType.Int32) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("name", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("displayName", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("salutation", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("firstName", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("middleName", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("lastName", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("nickName", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("suffix", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("gender_id", DataType.Int32) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("businessUnit_id", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("dateOfBirth", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },

                        // contactsTypes Array [1] 
                        new Field("contactTypes", DataType.Collection(DataType.String)) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },

                        new Field("accountingReference", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("referenceId", DataType.String) { IsKey = true, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },

                        // languageMappings Array [2] 
                        new Field("languageMappings", DataType.Collection(DataType.String)) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },

                        new Field("rating", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("primaryNumber", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },

                        // Numbers Array [3] 
                        new Field("numbers", DataType.Collection(DataType.String)) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        //new Field("primaryAddress", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },

                        // Testing GeographyPoint for 'lat' and 'lng' 
                        // new Field("lat", DataType.GeographyPoint) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        // new Field("lng", DataType.GeographyPoint) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },

                        // Addresses Array [4] 
                        new Field("addresses", DataType.Collection(DataType.String)) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        //new Field("primaryEmail", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },

                        // Emails Array [5]
                        new Field("emails", DataType.Collection(DataType.String)) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },

                        // Qualifications Array [6]
                        new Field("qualifications", DataType.Collection(DataType.String)) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },

                        // Eligibilties Array [7]
                        new Field("eligibilities", DataType.Collection(DataType.String)) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },

                        // CriteriaHierarchy Array [8]
                        new Field("criteriaHierarchy", DataType.Collection(DataType.String)) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },

                        new Field("hasTransportation", DataType.Boolean) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("hasChildren", DataType.Boolean) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("notes", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("companyName", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("website", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("region", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("countryOfOrigin", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("countryOfResidence", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("countryOfNationality", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("active", DataType.Boolean) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("activeNote", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("availability", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("experience", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("registeredTaxId", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("bankAccount", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("sortCode", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("iban", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("swift", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("eftId", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("eftName", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("paymentMethodId", DataType.Int32) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("paymentMethodName", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("paymentAccount", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("registeredTax", DataType.Boolean) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("registeredTaxIdDescription", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("employmentCategoryId", DataType.Int32) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("assignmentTierId", DataType.Int32) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("timeZone", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("ethnicity", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },

                        // Document Object
                        //new Field("document", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },

                        new Field("imagePath", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("outOfOffice", DataType.Boolean) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("disableUpcomingReminder", DataType.Boolean) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("disableCloseReminder", DataType.Boolean) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("disableConfirmReminder", DataType.Boolean) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("bankAccountDescription", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("timeWorked", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("activationDate", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("originalStartDate", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("datePhotoSentToPrinter", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("datePhotoSentToInterpreter", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("inductionDate", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("reActivationDate", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("iolNrcpdNumber", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("referralSource", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("refereeSourceName", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("recruiterName", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("taleoId", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("bankAccountReference", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },

                        // Status Object
                        //new Field("status", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },

                        new Field("disableConfirmationEmails", DataType.Boolean) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("disableOfferEmails", DataType.Boolean) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("disableAutoOffers", DataType.Boolean) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("currencyCodeId", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("currencySymbol", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("bankBranch", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },

                        // Services Array [9]
                        new Field("services", DataType.Collection(DataType.String)) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("enableAllServices", DataType.Boolean) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("isSynchronized", DataType.Boolean) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false },
                        new Field("lastSynchronizedDate", DataType.String) { IsKey = false, IsFacetable = false, IsFilterable = false, IsRetrievable = true, IsSearchable = false, IsSortable = false }
                        #endregion
                    }
                };

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
            // Scan the JSON files from the Interpreter Intelligence contacts and upload them to Azure Search.
            var indexOperations = new List<IndexAction>();

            string[] files = Directory.GetFiles(@"C:\Users\Ronan\source\repos\ArtGallery\ArtGallery\bin\Debug\json", "stephengregan.json", SearchOption.AllDirectories);
            int totalCounter = 0;

            try
            {
                foreach (var file in files)
                {
                    using (StreamReader jsonfile = File.OpenText(file))
                    {
                        #region Deserialize JSON
                        totalCounter++;
                        Document doc = new Document();
                        string json = jsonfile.ReadToEnd();

                        dynamic array = JsonConvert.DeserializeObject(json);

                        // Echovoice.JSON NuGet package

                        //string input = "[14,4,[14,\"data\"],[[5,\"10.186.122.15\"],[6,\"10.186.122.16\"]]]";
                        //string[] result = JSONDecoders.DecodeJSONArray(input);
                        //string[] result2 = JSONDecoders.DecodeJSONArray(result[3]);

                        // Deserialize the JSON
                        //dynamic contactTypes = JsonConvert.DeserializeObject(json);
                        //// Loop through it
                        //foreach (var s in contactTypes)
                        //{
                        //    // Print it out
                        //    Console.WriteLine(s);
                        //}

                        doc.Add("id", array["id"].Value);
                        doc.Add("versionValue", array["versionValue"].Value);
                        doc.Add("uuid", array["uuid"].Value);
                        doc.Add("createdBy", array["createdBy"].Value);
                        doc.Add("createdDate", array["createdDate"].Value);
                        doc.Add("lastModifiedBy", array["lastModifiedBy"].Value);
                        doc.Add("lastModifiedDate", array["lastModifiedDate"].Value);
                        doc.Add("company_id", array["company_id"] == null ? -1 : array["company_id"].Value);
                        doc.Add("name", array["name"].Value);
                        doc.Add("displayName", array["displayName"].Value);
                        doc.Add("salutation", array["salutation"].Value);
                        doc.Add("firstName", array["firstName"].Value);
                        doc.Add("middleName", array["middleName"].Value);
                        doc.Add("lastName", array["lastName"].Value);
                        doc.Add("nickName", array["nickName"].Value);
                        doc.Add("suffix", array["suffix"].Value);
                        doc.Add("gender_id", array["gender_id"] == null ? -1 : array["gender_id"].Value);
                        doc.Add("businessUnit_id", array["businessUnit_id"] == null ? "" : array["businessUnit_id"].Value);
                        doc.Add("dateOfBirth", array["dateOfBirth"].Value);

                        // ContactTypes Array [1]
                        //doc.Add("contactTypes", array["contactTypes"]);

                        doc.Add("accountingReference", array["accountingReference"].Value);
                        doc.Add("referenceId", array["referenceId"].Value);

                        // LanguageMappings Array [2]
                        //doc.Add("languageMappings", array["languageMappings"]);

                        doc.Add("rating", array["rating"] == null ? "" : array["rating"].Value);
                        //doc.Add("primaryNumber", array["primaryNumber"] == null ? "" : array["primaryNumber"].Value);

                        // Numbers Array [3]
                        //doc.Add("numbers", array["numbers"]);
                        //doc.Add("primaryAddress", array["primaryAddress"]);

                        // Testing GeographyPoint for 'lat' and 'lng' 
                        // doc.Add("lat", array["lat"]);
                        // doc.Add("lng", array["lng"]);

                        // Addresses Array [4]
                        //doc.Add("addresses", array["addresses"]);
                        //doc.Add("primaryEmail", array["primaryEmail"]);

                        // Emails Array [5]
                        //doc.Add("emails", array["emails"]);

                        // Qualifications Array [6]
                        //doc.Add("qualifications", array["qualifications"]);

                        // Eligibilities Array [7]
                        //doc.Add("eligibilities", array["eligibilities"]);

                        // CriteriaHierarchy Array [8]
                        //doc.Add("criteriaHierarchy", array["criteriaHierarchy"]);

                        doc.Add("hasTransportation", array["hasTransportation"] == null ? false : array["hasTransportation"].Value);
                        doc.Add("hasChildren", array["hasChildren"] == null ? false : array["hasChildren"].Value);
                        doc.Add("notes", array["notes"].Value);
                        doc.Add("companyName", array["companyName"].Value);
                        doc.Add("website", array["website"].Value);
                        doc.Add("region", array["region"].Value);
                        doc.Add("countryOfOrigin", array["countryOfOrigin"].Value);
                        doc.Add("countryOfResidence", array["countryOfResidence"].Value);
                        doc.Add("countryOfNationality", array["countryOfNationality"].Value);
                        doc.Add("active", array["active"] == null ? false : array["active"].Value);
                        doc.Add("activeNote", array["activeNote"].Value);
                        doc.Add("availability", array["availability"].Value);
                        doc.Add("experience", array["experience"].Value);
                        doc.Add("registeredTaxId", array["registeredTaxId"].Value);
                        doc.Add("bankAccount", array["bankAccount"].Value);
                        doc.Add("sortCode", array["sortCode"].Value);
                        doc.Add("iban", array["iban"].Value);
                        doc.Add("swift", array["swift"].Value);
                        doc.Add("eftId", array["eftId"] == null ? "" : array["eftId"].Value);
                        doc.Add("eftName", array["eftName"] == null ? "" : array["eftName"].Value);
                        doc.Add("paymentMethodId", array["paymentMethodId"].Value);
                        doc.Add("paymentMethodName", array["paymentMethodName"].Value);
                        doc.Add("paymentAccount", array["paymentAccount"] == null ? "" : array["paymentAccount"].Value);
                        doc.Add("registeredTax", array["registeredTax"] == null ? false : array["registeredTax"].Value);
                        doc.Add("registeredTaxIdDescription", array["registeredTaxIdDescription"].Value);
                        doc.Add("employmentCategoryId", array["employmentCategoryId"].Value);
                        doc.Add("assignmentTierId", array["assignmentTierId"].Value);
                        doc.Add("timeZone", array["timeZone"].Value);
                        doc.Add("ethnicity", array["ethnicity"].Value);

                        //doc.Add("document", array["document"]);

                        doc.Add("imagePath", array["imagePath"].Value);
                        doc.Add("outOfOffice", array["outOfOffice"] == null ? false : array["outOfOffice"].Value);
                        doc.Add("disableUpcomingReminder", array["disableUpcomingReminder"] == null ? false : array["disableUpcomingReminder"].Value);
                        doc.Add("disableCloseReminder", array["disableCloseReminder"] == null ? false : array["disableCloseReminder"].Value);
                        doc.Add("disableConfirmReminder", array["disableConfirmReminder"] == null ? false : array["disableConfirmReminder"].Value);
                        doc.Add("bankAccountDescription", array["bankAccountDescription"] == null ? "" : array["bankAccountDescription"].Value);
                        doc.Add("timeWorked", array["timeWorked"].Value);
                        doc.Add("activationDate", array["activationDate"] == null ? "" : array["activationDate"].Value);
                        doc.Add("originalStartDate", array["originalStartDate"] == null ? "" : array["originalStartDate"].Value);
                        doc.Add("datePhotoSentToPrinter", array["datePhotoSentToPrinter"] == null ? "" : array["datePhotoSentToPrinter"].Value);
                        doc.Add("datePhotoSentToInterpreter", array["datePhotoSentTpInterpreter"] == null ? "" : array["datePhotoSentToInterpreter"].Value);
                        doc.Add("inductionDate", array["inductionDate"] == null ? "" : array["inductionDate"].Value);
                        doc.Add("reActivationDate", array["reActivationDate"] == null ? "" : array["reActivationDate"].Value);
                        doc.Add("iolNrcpdNumber", array["iolNrcpdNumber"] == null ? "" : array["iolNrcpdNumber"].Value);
                        doc.Add("referralSource", array["referralSource"] == null ? "" : array["referralSource"].Value);
                        doc.Add("refereeSourceName", array["refereeSourceName"] == null ? "" : array["refereeSourceName"].Value);
                        doc.Add("recruiterName", array["recruiterName"] == null ? "" : array["recruiterName"].Value);
                        doc.Add("taleoId", array["taleoId"] == null ? "" : array["taleoId"].Value);
                        doc.Add("bankAccountReference", array["bankAccountreference"] == null ? "" : array["bankAccountReference"].Value);

                        //doc.Add("status", array["status"]);

                        doc.Add("disableConfirmationEmails", array["disableConfirmationEmails"] == null ? false : array["disableConfirmationEmails"].Value);
                        doc.Add("disableOfferEmails", array["disableOfferEmails"] == null ? false : array["disableOfferEmails"].Value);
                        doc.Add("disableAutoOffers", array["disableAutoOffers"] == null ? false : array["disableAutoOffers"].Value);
                        doc.Add("currencyCodeId", array["currencyCodeId"].Value);
                        doc.Add("currencySymbol", array["currencySymbol"] == null ? "" : array["currencySymbol"].Value);
                        doc.Add("bankBranch", array["bankBranch"] == null ? "" : array["bankBranch"].Value);

                        // Services Array [9]
                        //doc.Add("services", array["services"]);

                        doc.Add("enableAllServices", array["enableAllServices"] == null ? false : array["enableAllServices"].Value);
                        doc.Add("isSynchronized", array["isSynchronized"] == null ? false : array["isSynchronized"].Value);
                        doc.Add("lastSynchronizedDate", array["lastSynchronizedDate"] == null ? "" : array["lastSynchronizedDate"].Value);

                        // Parsing the Array's in the JSON
                        // ContactTypes Array [1]
                        if (array["contactTypes"] == null)
                        {
                            List<string> contributorList = new List<string>();

                            foreach (var item in array["contactTypes"])
                            {
                                contributorList.Add(((string)item["fc"]));
                            }

                            doc.Add("contactTypes", contributorList);
                        }

                        // LanguageMappings Array [2]
                        if (array["languageMappings"] == null)
                        {
                            List<string> contributorList = new List<string>();

                            foreach (var item in array["languageMappings"])
                            {
                                contributorList.Add(((string)item["fc"]));
                            }

                            doc.Add("languageMappings", contributorList);
                        }

                        // Numbers Array [3]
                        if (array["numbers"] == null)
                        {
                            List<string> contributorList = new List<string>();

                            foreach (var item in array["numbers"])
                            {
                                contributorList.Add(((string)item["fc"]));
                            }

                            doc.Add("numbers", contributorList);
                        }

                        // Addresses Array [4]
                        if (array["addresses"] == null)
                        {
                            List<string> contributorList = new List<string>();

                            foreach (var item in array["addresses"])
                            {
                                contributorList.Add(((string)item["fc"]));
                            }

                            doc.Add("addresses", contributorList);
                        }

                        // Emails Array [5]
                        if (array["emails"] == null)
                        {
                            List<string> contributorList = new List<string>();

                            foreach (var item in array["emails"])
                            {
                                contributorList.Add(((string)item["fc"]));
                            }

                            doc.Add("emails", contributorList);
                        }

                        // Qualifications Array [6]
                        if (array["qualifications"] == null)
                        {
                            List<string> contributorList = new List<string>();

                            foreach (var item in array["qualifications"])
                            {
                                contributorList.Add(((string)item["fc"]));
                            }

                            doc.Add("qualifications", contributorList);
                        }

                        // Eligibilitites Array [7]
                        if (array["eligibilities"] == null)
                        {
                            List<string> contributorList = new List<string>();

                            foreach (var item in array["eligibilities"])
                            {
                                contributorList.Add(((string)item["fc"]));
                            }

                            doc.Add("eligibilities", contributorList);
                        }

                        // CriteriaHierarchy Array [8]
                        if (array["criteriaHierarchy"] == null)
                        {
                            List<string> contributorList = new List<string>();

                            foreach (var item in array["criteriaHierarchy"])
                            {
                                contributorList.Add(((string)item["fc"]));
                            }

                            doc.Add("criteriaHierarchy", contributorList);
                        }

                        // Services Array [9]
                        if (array["services"] == null)
                        {
                            List<string> contributorList = new List<string>();

                            foreach (var item in array["services"])
                            {
                                contributorList.Add(((string)item["fc"]));
                            }

                            doc.Add("services", contributorList);
                        }
                        #endregion

                        indexOperations.Add(IndexAction.Upload(doc));

                        if (indexOperations.Count >= 500)
                        {
                            Console.WriteLine("Writing {0} documents of {1} total documents...", indexOperations.Count, totalCounter);
                            _searchIndexClient.Documents.Index(new IndexBatch(indexOperations));
                            indexOperations.Clear();
                        }
                    }
                }

                if (indexOperations.Count >= 0)
                {
                    Console.WriteLine("Writing {0} documents of {1} total documents...", indexOperations.Count, totalCounter);
                    _searchIndexClient.Documents.Index(new IndexBatch(indexOperations));
                }
            }
            catch (IndexBatchException ex)
            {
                // Sometimes when the search service is under load, indexing will fail for some of the documents in the batch.
                Console.WriteLine("Failed to index some of the documents: {0}", String.Join(", ", ex.IndexingResults.Where(r => !r.Succeeded).Select(r => r.Key)));
            }
        }
        #endregion
    }
}