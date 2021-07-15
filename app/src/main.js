const config = {
	apiUrl: import.meta.env.VITE_API_URL,
	newDocumentUrl: import.meta.env.VITE_API_NEW_DOCUMENT_URL,
};

window.addEventListener("DOMContentLoaded", async () => {
	// Initialise an empty document and update the form action to the new document URL.
 	// Default behaviour (e.g. when JS is disabled) is to create a new document on every submit.
	// If initialisation successful, submit always updates the same document.
	var newDocumentUri = await initialiseNewDocument();
	updateFormUrl(newDocumentUri);
});

async function initialiseNewDocument() {
	const newDocument = await fetch(config.newDocumentUrl, { method: "post" });

	// cannot read Content-Location header (because of CORS) so read URI in response body
	const newDocumentUri = await newDocument.text();
	return newDocumentUri;
}

function updateFormUrl(actionUri) {
	var forms = document.getElementsByTagName("form");
	for (let i = 0; i < forms.length; ++i) {
		forms[i].action = config.apiUrl + actionUri;
	}
}
