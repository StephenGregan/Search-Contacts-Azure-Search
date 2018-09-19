var azureSearchQueryKey = "[SearchServiceQueryKey]";
var inSearch= false;
var q = "*";
var lastModifiedByFacet = '';
var createdByFacet = '';
var countryOfOriginFacet = '';
var activeFacet = '';
var hasTransportationFacet = '';
var currentPage = 1;

function execSearch()
{
	// Execute a search to lookup
	if (inSearch == false) {
		inSearch = true;
		var q = $("#q").val();
		var searchQuery = encodeURIComponent(q);

		if (q.length = 0) 
			searchQuery = '*';

		if (searchQuery.length == 0)
			searchQuery = '*';

		// if(lastModifiedByFacet > 0)
		// 	searchQuery += '&$filter=lastModifiedBy/any(t: t eq \'' + encodeURIComponent(lastModifiedByFacet) + '\')';

		// if(createdByFacet > 0){
		// 	if(lastModifedByFacet.length == 0)
		// 		searchQuery += '&$filter=';
		// 	else
		// 		searchQuery += ' and ';
		// 	searchQuery += '&$filter=createdBy/any(t: t eq \'' + encodeURIComponent(createdByFacet) + '\')';
		// }

		if(activeFacet > 0)
			searchQuery += '&$filter=active/any(t: t eq \'' + encodeURIComponent(activeFacet) + '\')';
		
		if(createdByFacet > 0)
			searchQuery += '&$filter=createdBy/any(t: t eq \'' + encodeURIComponent(createdByFacet) + '\')';

		if(lastModifiedByFacet > 0)
			searchQuery += '&$filter=lastModifiedBy/any(t: t eq \'' + encodeURIComponent(lastModifiedByFacet) + '\')';

		if(countryOfOriginFacet > 0)
			searchQuery += '&$filter=countryOfOrigin/any(t: t eq \'' + encodeURIComponent(countryOfOriginFacet) + '\')';

		if(hasTransportationFacet > 0)
			searchQuery += '&$filter=hasTransportation/any(t: t eq \'' + encodeURIComponent(hasTransportationFacet) + '\')';

		var searchAPI = "https://ronansearch.search.windows.net/indexes/index/docs?$skip=" + (currentPage-1).toString() + "&$top=5&$select=imagePath,displayName,id"
				searchAPI += ",uuid,createdBy,createdDate,lastModifiedBy,lastModifiedDate,firstName,active,middleName,lastName,accountingReference,countryOfOrigin,"
				searchAPI += "disableUpcomingReminder,disableCloseReminder,disableConfirmReminder,hasTransportation,hasChildren,companyName,website,bankAccount,"
				searchAPI += "registeredTax,registeredTaxIdDescription,sortCode,iban,swift,notes,activeNote,contactTypes,languageMappings,numbers,addresses,emails,"
				searchAPI += "qualifications,eligibilities,criteriaHierarchy,services,primaryNumber,primaryAddress,primaryEmail,document,status&api-version=2015-02-28"
				searchAPI += "&highlight=displayName&$count=true&facet=active&facet=lastModifiedBy&facet=createdBy&facet=countryOfOrigin&facet=hasTransportation&search=" + searchQuery;

		$.ajax({
			url: searchAPI,
			beforeSend: function (request) {
				request.setRequestHeader("Access-Control-Allow-Origin", "*");
				request.setRequestHeader("api-key", azureSearchQueryKey);
				request.setRequestHeader("Content-Type", "application/json");
				request.setRequestHeader("Accept", "application/json; odata.metadata=none");
			},
			type: "GET",
			success: function (data) {

				$("#mediaContainer").html('');
				$("#activeContainer").html('');
				$("#countryOfOriginContainer").html('');
				$("#lastModifiedByContainer").html('');
				$("#createdByContainer").html('');
				$("#hasTransportationContainer").html('');
				$("#jobs-count").html(data.Count);

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
						var divContent = '<div class="row">';
								divContent += '<div class="col-md-4">';
								divContent += '<br><p><a href="' + imagePath + '"><img class="img-responsive" src="' + imagePath + '" alt=""></a></p>';
								divContent += '<h2>Contact Information</h2>';
								divContent += '<p><b>Name:</b> ' + displayName + '</p><p><b>Id:</b> ' + id + '</p>';
								divContent += '<p><b>Uuid:</b> ' + uuid + '</p><p><b>Created By:</b> ' + createdBy + '</p>';
								divContent += '<p><b>Created Date:</b> ' + createdDate + '</p><p><b>Last Modified By:</b> ' + lastModifiedBy + '</p>';
								divContent += '<p><b>Last Modified Date:</b> ' + lastModifiedDate + '</p><p><b>First Name:</b> ' + firstName + '</p>';
								divContent += '<p><b>Active:</b> ' + active + '</p><p><b>Middle Name:</b> ' + middleName + '</p>';
								divContent += '<p><b>Last Name:</b> ' + lastName + '</p><p><b>accounting Reference:</b> ' + accountingReference + '</p>';
								divContent += '<p><b>Country Of Origin:</b> ' + countryOfOrigin + '</p>';
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

						$("#mediaContainer").append(divContent);
				}

				var active = '';
				for (var item in data["@search.facets"].active)
				{
					if (activeFacet != data["@search.facets"].active[item].value) {
						$( "#activeContainer" ).append( '<li><a href="javascript:void(0);" onclick="setActiveFacet(\'' + data["@search.facets"].active[item].value + '\');">' + data["@search.facets"].active[item].value + ' (' + data["@search.facets"].active[item].count + ')</a></li>' );
					}
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

				var hasTransportation = '';
				for (var item in data["@search.facets"].hasTransportation)
				{
					if (hasTransportationFacet != data["@search.facets"].hasTransportation[item].value) {
						$( "#hasTransportationContainer" ).append( '<li><a href="javascript:void(0);" onclick="setHasTransportationFacet(\'' + data["@search.facets"].hasTransportation[item].value + '\');">' + data["@search.facets"].hasTransportation[item].value + ' (' + data["@search.facets"].hasTransportation[item].count + ')</a></li>' );
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

function setActiveFacet(facet)
{
	// User clicked on a subject facet
	activeFacet = facet;
	if (facet != '')
		$("#currentActive").html(facet + '<a href="javascript:void(0);" onclick="setActiveFacet(\'\');"> [X]</a>');
	else
		$("#currentActive").html('');
	execSearch();
}

function setLastModifiedByFacet(facet)
{
	// User clicked on a subject facet
	lastModifedByFacet=facet;
	if (facet != '')
		$("#currentLastModifedBy").html(facet + '<a href="javascript:void(0);" onclick="setLastModifedByFacet(\'\');"> [X]</a>');
	else
		$("#currentLastModifedBy").html('');
	execSearch();
}

function setCreatedByFacet(facet)
{
	// User clicked on a subject facet
	createdByFacet=facet;
	if (facet != '')
		$("#currentCreatedBy").html(facet + '<a href="javascript:void(0);" onclick="setCreatedByFacet(\'\');"> [X]</a>');
	else
		$("#currentCreatedBy").html('');
	execSearch();
}

function setCountryOfOriginFacet(facet)
{
	// User clicked on a subject facet
	countryOfOriginFacet=facet;
	if (facet != '')
		$("#currentCountryOfOrigin").html(facet + '<a href="javascript:void(0);" onclick="setCountryOfOriginFacet(\'\');"> [X]</a>');
	else
		$("#currentCountryOfOrigin").html('');
	execSearch();
}

function setHasTransportationFacet(facet)
{
	// User clicked on a subject facet
	hasTransportationFacet=facet;
	if (facet != '')
		$("#currentHasTransportation").html(facet + '<a href="javascript:void(0);" onclick="setHasTransportationFacet(\'\');"> [X]</a>');
	else
		$("#currentHasTransportation").html('');
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


