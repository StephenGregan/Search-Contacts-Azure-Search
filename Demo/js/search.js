var azureSearchQueryKey = "";
var inSearch= false;
var q = "*";
// Test Gallery
var subjectFacet = "";
// Test Gallery
var contributorFacet = "";
var currentPage = 1;

function execSearch()
{
	// Execute a search to lookup
	if (inSearch == false) {
		inSearch = true;
		var q = $("#q").val();
		var searchQuery = encodeURIComponent(q);

		if (q.length = 0) 
			searchQuery = "*";
		if (searchQuery.length == 0)
			searchQuery = "*";

		var searchAPI = "https://ronansearch.search.windows.net/indexes/index/docs?$skip=" + (currentPage-1).toString() + "&$top=10&$select=imagePath,displayName,id,createdBy,createdDate,lastModifiedBy,lastModifiedDate,firstName,active,middleName,lastName,accountingReference,countryOfOrigin,disableUpcomingReminder,disableCloseReminder,disableConfirmReminder,sortCode,iban,swift,notes,activeNote,contactTypes,languageMappings,numbers,addresses,emails,qualifications,eligibilities,criteriaHierarchy,services&api-version=2015-02-28&$count=true&search=" + searchQuery;

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
					// var rating = data.value[item].rating;
					var countryOfOrigin = data.value[item].countryOfOrigin;
					var bankAccount = data.value[item].bankAccount;
					var ethnicity = data.value[item].ethnicity;
					var disableUpcomingReminder = data.value[item].disableUpcomingReminder;
					var disableCloseReminder = data.value[item].disableCloseReminder;
					var disableConfirmReminder = data.value[item].disableConfirmReminder;
					// var bankAccountDescription = data.value[item].bankAccountDescription;
					// var iolNrcpdNumber = data.value[item].iolNrcpdNumber;
					// var taleoId = data.value[item].taleoId;
					var activeNote = data.value[item].activeNote;
					var sortCode = data.value[item].sortCode;
					var iban = data.value[item].iban;
					var swift = data.value[item].swift;
					var notes = data.value[item].notes;
					var imagePath = data.value[item].imagePath;
					var contactTypes = data.value[item].contactTypes;
					var languageMappings = data.value[item].languageMappings;
					var numbers = data.value[item].numbers;
					var addresses = data.value[item].addresses;
					var emails = data.value[item].emails;
					var qualifications = data.value[item].qualifications;
					var eligibilities = data.value[item].eligibilities;
					var criteriaHierarchy = data.value[item].criteriaHierarchy;
					var services = data.value[item].services;
					
					// You will get undefined for everything if you dont specify all the fields on the web page in searchApi
					var divContent = '<div class="row"><div class="col-md-4">';
							divContent += '<p><a href="' + imagePath + '"><img class="img-responsive" src="' + imagePath + '" alt=""></p><h2>Contact Information:</h2><p><b>Name:</b> ' + displayName + '</p><p><b>Id:</b> ' + id + '</p><p><b>Created By:</b> ' + createdBy + '</p><p><b>Created Date:</b> ' + createdDate + '</p><p><b>Last Modified By:</b> ' + lastModifiedBy + '</p>';
							divContent += '<p><b>Last Modified Date:</b> ' + lastModifiedDate + '</p><p><b>First Name:</b> ' + firstName + '</p><p><b>Active:</b> ' + active + '</p><p><b>Middle Name:</b> ' + middleName + '</p>';
							divContent += '<p><b>Last Name:</b> ' + lastName + '</p><p><b>accounting Reference:</b> ' + accountingReference + '</p><p><b>Country Of Origin:</b> ' + countryOfOrigin + '</p>';
							divContent += '<p><b>disableUpcomingReminder:</b> ' + disableUpcomingReminder + '</p><p><b>disableCloseReminder:</b> ' + disableCloseReminder + '</p><p><b>disableConfirmReminder:</b> ' + disableConfirmReminder + '</p>';
							divContent += '<p><b>Sort Code:</b> ' + sortCode + '</p><p><b>I Ban:</b> ' + iban + '</p><p><b>Swift:</b> ' + swift + '</p><p><b>Notes:</b> ' + notes + '</p><p><b>Active Note:</b> ' + activeNote + '</p><br></div>';
							divContent += '<br><div class="col-md-8"><h2>Arrays:</h2><p><b>Contact Types:</b> ' + contactTypes + '</p><p><b>Language Mappings:</b> ' + languageMappings + '</p>';
							divContent += '<p><b>Numbers:</b> ' + numbers + '</p><p><b>Addresses:</b> ' + addresses + '</p><p><b>Emails:</b> ' + emails + '</p><p><b>Qualifications:</b> ' + qualifications + '</p>';
							divContent += '<p><b>Eligibilities:</b> ' + eligibilities + '</p><p><b>Criteria Hierarchy:</b> ' + criteriaHierarchy + '</p><p><b>Services:</b> ' + services + '</p></div>';

					$("#mediaContainer").append(divContent);
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
	
function ChangePage(page)
{
	// User clicked on the pagination
	currentPage = page;
	execSearch();
}


