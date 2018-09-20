// You need the Azure Search Query Key to search the index.  You can add/delete query keys in the Azure Portal.
// A good idea I think would be to put the query keys in SQL and validate them when a user enters the correct 
// query key from a login page similar to a username and password login.  The login page is included in the 
// Demo folder.  For instance if the user enters the correct query key let them search the index, if not link 
// them a email to enquire about how to get a query key.
var azureSearchQueryKey = "";
// Default inSearch to false.
var inSearch = false;
// Search Everything
var q = '*';
// All the facets for the index.
var lastModifiedByFacet = '';
var createdByFacet = '';
var countryOfOriginFacet = '';
var activeFacet = '';
var hasTransportationFacet = '';
var numbersFacet = '';
var qualificationsFacet = '';
// Set the current page to page 1.
var currentPage = 1;

// The execSearch function
function execSearch()
{
	// Execute a search to lookup 
	if (inSearch == false) {
		inSearch= true;
		var q = $("#q").val();
		var searchQuery = encodeURIComponent(q);
		
		if (q.length = 0)
			searchQuery = '*';

		if (searchQuery.length == 0)
			searchQuery = '*';

		if (lastModifiedByFacet.length > 0)
			searchQuery += '&$filter=lastModifiedBy/any(t: t eq \'' + encodeURIComponent(lastModifiedByFacet) + '\')';

		if (createdByFacet.length > 0)
			searchQuery += '&$filter=createdBy/any(t: t eq \'' + encodeURIComponent(createdByFacet) + '\')';

		if (countryOfOriginFacet.length > 0)
			searchQuery += '&$filter=countryOfOrigin/any(t: t eq \'' + encodeURIComponent(countryOfOriginFacet) + '\')';

		if (activeFacet.length > 0)
			searchQuery += '&$filter=active/any(t: t eq \'' + encodeURIComponent(activeFacet) + '\')';

		if (hasTransportationFacet.length > 0)
			searchQuery += '&$filter=hasTransportation/any(t: t eq \'' + encodeURIComponent(hasTransportationFacet) + '\')';

		if (numbersFacet.length > 0)
			searchQuery += '&$filter=numbers/any(t: t eq \'' + encodeURIComponent(numbersFacet) + '\')';

		if (qualificationsFacet.length > 0)
			searchQuery += '&$filter=qualifications/any(t: t eq \'' + encodeURIComponent(qualificationsFacet) + '\')';

		// Set up all the url, fields, facets, hit highlighting(currently not implemented) and contact page limit.  I have set it to 5 documents per page.
		// Functionality that would enhance the search experience would be to implement hit highlighting, this sets up when you make a search eg Adam you will get 
		// Adam highlighted.
		var searchAPI = "https://ronansearch.search.windows.net/indexes/contactsindex/docs?$skip=" + (currentPage-1).toString() + "&$top=5&$select=imagePath,";
				searchAPI += "displayName,id,uuid,createdBy,createdDate,lastModifiedBy,lastModifiedDate,firstName,active,middleName,lastName,dateOfBirth,accountingReference,";
				searchAPI += "countryOfOrigin,disableUpcomingReminder,disableCloseReminder,disableConfirmReminder,hasTransportation,hasChildren,companyName,website,";
				searchAPI += "bankAccount,registeredTax,registeredTaxIdDescription,sortCode,iban,swift,notes,activeNote,contactTypes,languageMappings,numbers,addresses,";
				searchAPI += "emails,qualifications,eligibilities,criteriaHierarchy,services,primaryNumber,primaryAddress,primaryEmail,document,status&api-version=2015-02-";
				searchAPI += "28&$count=true&facet=active&facet=lastModifiedBy&facet=createdBy&facet=countryOfOrigin&facet=hasTransportation&facet=numbers&facet=qualifications";
				searchAPI += "&search=" + searchQuery;
		$.ajax({
			url: searchAPI,
			beforeSend: function (request) {
				// You must allow origins if you want to display the documents 
				// indexed on to a web page.  This '*' specifies that we want to
				// enable CORS for everything.  This was used strictly by me
				// and you would never want to do this and it leaves room to
				// get exploited.  Information about CORS: 
				request.setRequestHeader("Access-Control-Allow-Origin", "*");
				// Setting up the Api/Query key
				request.setRequestHeader("api-key", azureSearchQueryKey);
				// The type is type JSON (JavaScript Object Notation)
				// Link:
				request.setRequestHeader("Content-Type", "application/json");
				// This specifies that we should only accept the type JSON
				request.setRequestHeader("Accept", "application/json; odata.metadata=none");
			},
			// This is a GET request where we get all the content
			// we want from the created index
			type: "GET",
			success: function (data) {
				$("#mediaContainer").html('');
				$("#lastModifiedByContainer").html('');
				$("#createdByContainer").html('');
				$("#countryOfOriginContainer").html('');
				$("#activeContainer").html('');
				$("#hasTransportationContainer").html('');
				$("#numbersContainer").html('');
				$("#qualificationsContainer").html('');

				// Declare the item variable and loop throught the data
				// I have taken all the fields that I think are valid.
				// You can add or remove what you like.
				for (var item in data.value) {
						var id = data.value[item].id;
						var uuid = data.value[item].uuid;
						var versionValue = data.value[item].versionValue;
						var createdBy = data.value[item].createdBy;
						var createdDate = data.value[item].createdDate;
						var lastModifiedBy = data.value[item].lastModifiedBy;
						var lastModifiedDate = data.value[item].lastModifiedDate;
						var firstName = data.value[item].firstName;
						var displayName = data.value[item].displayName;
						var active = data.value[item].active;
						var dateOfBirth = data.value[item].dateOfBirth;
						var middleName = data.value[item].middleName;
						var lastName = data.value[item].lastName;
						var accountingReference = data.value[item].accountingReference;
						var countryOfOrigin = data.value[item].countryOfOrigin;
						var bankAccount = data.value[item].bankAccount;
						var ethnicity = data.value[item].ethnicity;
						var disableUpcomingReminder = data.value[item].disableUpcomingReminder;
						var disableCloseReminder = data.value[item].disableCloseReminder;
						var disableConfirmReminder = data.value[item].disableConfirmReminder;
						var activeNote = data.value[item].activeNote;
						var sortCode = data.value[item].sortCode;
						var iban = data.value[item].iban;
						var swift = data.value[item].swift;
						var notes = data.value[item].notes;
						var imagePath = data.value[item].imagePath;
						var uuid = data.value[item].uuid;
						var hasTransportation = data.value[item].hasTransportation;
						var hasChildren = data.value[item].hasChildren;
						var companyName = data.value[item].companyName;
						var website = data.value[item].website;
						var bankAccount = data.value[item].bankAccount;
						var registeredTax = data.value[item].registeredTax;
						var registeredTaxIdDescription = data.value[item].registeredTaxIdDescription;

						// Objects
						var primaryNumber = data.value[item].primaryNumber;
						var primaryAddress = data.value[item].primaryAddress;
						var primaryEmail = data.value[item].primaryEmail;
						var document = data.value[item].document;
						var status = data.value[item].status;

						// Arrays
						var contactTypes = data.value[item].contactTypes;
						var languageMappings = data.value[item].languageMappings;
						var numbers = data.value[item].numbers;
						var addresses = data.value[item].addresses;
						var emails = data.value[item].emails;
						var qualifications = data.value[item].qualifications;
						var eligibilities = data.value[item].eligibilities;
						var criteriaHierarchy = data.value[item].criteriaHierarchy;
						var services = data.value[item].services;

						// You will get undefined for everything if you dont specify all the fields on the web page in the searchAPI variable
						// This displays how everything will look when it is displayed on the web page.  I have tried to make it as readable
						// as possible.  Please break it down into sections if it is confusing at first.
						var divContent = '<div class="row">';
								divContent += '<div class="col-md-4">';
								divContent += '<br><p><a href="' + imagePath + '"><img class="img-responsive" src="' + imagePath + '" alt=""></a></p>';
								divContent += '<h2>Contact Information</h2>';
								divContent += '<p><b>Name:</b> ' + displayName + '</p><p><b>Id:</b> ' + id + '</p>';
								divContent += '<p><b>Uuid:</b> ' + uuid + '</p><p><b>Created By:</b> ' + createdBy + '</p>';
								divContent += '<p><b>Created Date:</b> ' + createdDate + '</p><p><b>Last Modified By:</b> ' + lastModifiedBy + '</p>';
								divContent += '<p><b>Last Modified Date:</b> ' + lastModifiedDate + '</p><p><b>First Name:</b> ' + firstName + '</p>';
								divContent += '<p><b>Active:</b> ' + active + '</p><p><b>Middle Name:</b> ' + middleName + '</p>';
								divContent += '<p><b>Last Name:</b> ' + lastName + '</p><p><b>Date of Birth:</b> ' + dateOfBirth + '</p>';
								divContent += '<p><b>accounting Reference:</b> ' + accountingReference + '</p><p><b>Country Of Origin:</b> ' + countryOfOrigin + '</p>';
								divContent += '<p><b>disableUpcomingReminder:</b> ' + disableUpcomingReminder + '</p>';
								divContent += '<p><b>disableCloseReminder:</b> ' + disableCloseReminder + '</p>';
								divContent += '<p><b>disableConfirmReminder:</b> ' + disableConfirmReminder + '</p>';
								divContent += '<p><b>Has Transportation By:</b> ' + hasTransportation + '</p>';
								divContent += '<p><b>Has Children:</b> ' + hasChildren + '</p><p><b>Company Name:</b> ' + companyName + '</p>';
								divContent += '<p><b>Website:</b> ' + website + '</p>';
								divContent += '<p><b>Bank Account:</b> ' + bankAccount + '</p><p><b>Registered Tax:</b> ' + registeredTax + '</p>';
								divContent += '<p><b>Registered Tax Id Description:</b> ' + registeredTaxIdDescription + '</p>';
								divContent += '<p><b>Sort Code:</b> ' + sortCode + '</p><p><b>I Ban:</b> ' + iban + '</p>';
								divContent += '<p><b>Swift:</b> ' + swift + '</p><p><b>Notes:</b> ' + notes + '</p>';
								divContent += '<p><b>Active Note:</b> ' + activeNote + '</p><br></div>';
								divContent += '<br><div class="col-md-8"><h2>Arrays</h2><p><b>Contact Types:</b> ' + contactTypes + '</p>';
								divContent += '<p><b>Language Mappings:</b> ' + languageMappings + '</p>';
								divContent += '<p><b>Numbers:</b> ' + numbers + '</p><p><b>Addresses:</b> ' + addresses + '</p>';
								divContent += '<p><b>Emails:</b> ' + emails + '</p><p><b>Qualifications:</b> ' + qualifications + '</p>';
								divContent += '<p><b>Eligibilities:</b> ' + eligibilities + '</p>';
								divContent += '<p><b>Criteria Hierarchy:</b> ' + criteriaHierarchy + '</p>';
								divContent += '<p><b>Services:</b> ' + services + '</p><h2>Objects</h2>';
								divContent += '<p><b>Primary Number:</b> ' + primaryNumber + '</p>';
								divContent += '<p><b>Primary Address:</b> ' + primaryAddress + '</p><p><b>Primary Email:</b> ' + primaryEmail + '</p>';
								divContent += '<p><b>Document:</b> ' + document + '</p><p><b>Status:</b> ' + status + '</p></div>';
						// Append the above content into the mediaContainer
						$("#mediaContainer").append(divContent);
				}	

				var lastModifiedBy = '';
				for (var item in data["@search.facets"].lastModifiedBy)
				{
					if (lastModifiedByFacet != data["@search.facets"].lastModifiedBy[item].value) {
						$( "#lastModifiedByContainer" ).append( '<li><a href="javascript:void(0);" onclick="setLastModifiedByFacet(\'' + data["@search.facets"].lastModifiedBy[item].value + '\');">' + data["@search.facets"].lastModifiedBy[item].value + ' (' + data["@search.facets"].lastModifiedBy[item].count + ')</a></li>' );
					}
				}

				var createdBy = '';
				for (var item in data["@search.facets"].createdBy)
				{
					if (createdByFacet != data["@search.facets"].createdBy[item].value) {
						$( "#createdByContainer" ).append( '<li><a href="javascript:void(0);" onclick="setCreatedByFacet(\'' + data["@search.facets"].createdBy[item].value + '\');">' + data["@search.facets"].createdBy[item].value + ' (' + data["@search.facets"].createdBy[item].count + ')</a></li>' );
					}
				}

				var countryOfOrigin = '';
				for (var item in data["@search.facets"].countryOfOrigin)
				{
					if (countryOfOriginFacet != data["@search.facets"].countryOfOrigin[item].value) {
						$( "#countryOfOriginContainer" ).append( '<li><a href="javascript:void(0);" onclick="setCountryOfOriginFacet(\'' + data["@search.facets"].countryOfOrigin[item].value + '\');">' + data["@search.facets"].countryOfOrigin[item].value + ' (' + data["@search.facets"].countryOfOrigin[item].count + ')</a></li>' );
					}
				}

				var active = '';
				for (var item in data["@search.facets"].active)
				{
					if (activeFacet != data["@search.facets"].active[item].value) {
						$( "#activeContainer" ).append( '<li><a href="javascript:void(0);" onclick="setActiveFacet(\'' + data["@search.facets"].active[item].value + '\');">' + data["@search.facets"].active[item].value + ' (' + data["@search.facets"].active[item].count + ')</a></li>' );
					}
				}

				var hasTransportation = '';
				for (var item in data["@search.facets"].hasTransportation)
				{
					if (hasTransportationFacet != data["@search.facets"].hasTransportation[item].value) {
						$( "#hasTransportationContainer" ).append( '<li><a href="javascript:void(0);" onclick="setHasTransportationFacet(\'' + data["@search.facets"].hasTransportation[item].value + '\');">' + data["@search.facets"].hasTransportation[item].value + ' (' + data["@search.facets"].hasTransportation[item].count + ')</a></li>' );
					}
				}

				var numbers = '';
				for (var item in data["@search.facets"].numbers)
				{
					if (numbersFacet != data["@search.facets"].numbers[item].value) {
						$( "#numbersContainer" ).append( '<li><a href="javascript:void(0);" onclick="setNumbersFacet(\'' + data["@search.facets"].numbers[item].value + '\');">' + data["@search.facets"].numbers[item].value + ' (' + data["@search.facets"].numbers[item].count + ')</a></li>' );
					}
				}

				var qualifications = '';
				for (var item in data["@search.facets"].qualifications)
				{
					if (qualificationsFacet != data["@search.facets"].qualifications[item].value) {
						$( "#qualificationsContainer" ).append( '<li><a href="javascript:void(0);" onclick="setQualificationsFacet(\'' + data["@search.facets"].qualifications[item].value + '\');">' + data["@search.facets"].qualifications[item].value + ' (' + data["@search.facets"].qualifications[item].count + ')</a></li>' );
					}
				}

				// Update Pagination
				UpdatePagination(data["@search.count"]);
			}
		}).done(function (data) {
			inSearch = false;
			// Check if the user changed the search term since this completed
			if (q != $("#q").val())
				execSearch();
		});
	}	
}

function setLastModifiedByFacet(facet) {
	// User clicked on a lastModifiedBy facet
	lastModifedByFacet = facet;
	if (facet != '')
		$("#currentLastModifedBy").html(facet + '<a href="javascript:void(0);" onclick="setLastModifiedByFacet(\'\');"> [X]</a>');
	else
		$("#currentLastModifedBy").html('');
	execSearch();
}

function setCreatedByFacet(facet) {
	// User clicked on a createdBy facet
	createdByFacet = facet;
	if (facet != '')
		$("#currentCreatedBy").html(facet + '<a href="javascript:void(0);" onclick="setCreatedByFacet(\'\');"> [X]</a>');
	else
		$("#currentCreatedBy").html('');
	execSearch();
}

function setCountryOfOriginFacet(facet) {
	// User clicked on a countryOfOrigin facet
	countryOfOriginFacet = facet;
	if (facet != '')
		$("#currentCountryOfOrigin").html(facet + '<a href="javascript:void(0);" onclick="setCountryOfOriginFacet(\'\');"> [X]</a>');
	else
		$("#currentCountryOfOrigin").html('');
	execSearch();
}

function setActiveFacet(facet) {
	// User clicked on a active facet
	activeFacet = facet;
	if (facet != '')
		$("#currentActive").html(facet + '<a href="javascript:void(0);" onclick="setActiveFacet(\'\');"> [X]</a>');
	else
		$("#currentActive").html('');
	execSearch();
}

function setHasTransportationFacet(facet) {
	// User clicked on a hasTransportation facet
	hasTransportationFacet = facet;
	if (facet != '')
		$("#currentHasTransportation").html(facet + '<a href="javascript:void(0);" onclick="setHasTransportationFacet(\'\');"> [X]</a>');
	else
		$("#currentHasTransportation").html('');
	execSearch();
}

function setNumbersFacet(facet) {
	// User clicked on a numbers facet
	numbersFacet = facet;
	if (facet != '')
		$("#currentNumbers").html(facet + '<a href="javascript:void(0);" onclick="setNumbersFacet(\'\');"> [X]</a>');
	else
		$("#currentNumbers").html('');
	execSearch();
}

function setQualificationsFacet(facet) {
	// User clicked on a qualifications facet
	qualificationsFacet = facet;
	if (facet != '')
		$("#currentQualifications").html(facet + '<a href="javascript:void(0);" onclick="setQualificationsFacet(\'\');"> [X]</a>');
	else
		$("#currentQualifications").html('');
	execSearch();
}

function UpdatePagination(docCount) {
	// Update the pagination
	var totalPages = Math.round(docCount / 10);
	// Set a max of 5 items and set the current page in middle of pages
	var startPage = currentPage;
	if ((startPage == 1) || (startPage == 2))
		startPage = 1;
	else
		startPage -= 2;

	var maxPage = startPage + 5;
	if (totalPages < maxPage)
		maxPage = totalPages + 1;
	var backPage = parseInt(currentPage) - 1;
	if (backPage < 1)
		backPage = 1;
	var forwardPage = parseInt(currentPage) + 1;

	var htmlString = '<li class=""><a href="javascript:void(0);" onclick="ChangePage(' + backPage +');" aria-label="Previous" id="PagingPrevious"><span aria-hidden="true">&laquo;</span></a></li>';
	for (var i = startPage; i < maxPage; i++) {
		if (i == currentPage)
			htmlString += '<li  class="active"><a href="#">' + i + '</a></li>';
		else
			htmlString += '<li><a href="javascript:void(0)" onclick="ChangePage(\'' + parseInt(i) + '\')">' + i + '</a></li>';
	}

	htmlString += '<li class=""><a href="javascript:void(0);" onclick="ChangePage(' + forwardPage + ');" aria-label="Next" id="PagingNext"><span aria-hidden="true">&raquo;</span></a></li>';
	$("#pagination").html(htmlString);
}
	
function ChangePage(page) {
	// User clicked on the pagination
	currentPage = page;
	execSearch();
}


