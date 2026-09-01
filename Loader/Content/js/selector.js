console.log("selector.js loaded");

$(".launchButton").click(function() 
{
	console.log("launchButton clicked");

	// Get & store the selected option
	var selectedOption = $("#selectionBox select option:selected").val();
	console.log("selectedOption:", selectedOption);

	// Store the checkstate of the checkbox.
	var checkState = $("#saveSelectionSwitch input[type='checkbox']").is(':checked');
	console.log("checkState:", checkState);

	if (typeof selectorObject === "undefined") {
		console.error("selectorObject is undefined! JS binding did not register.");
		return;
	}

	console.log("calling selectorObject.handleLaunch...");

	// Handle the launch via the handleLaunch method.
	selectorObject.handleLaunch(selectedOption, checkState);

	console.log("selectorObject.handleLaunch call completed");
});

/* 
	Remember selection
*/

console.log("checking selectorObject before handleRememberSelection:", typeof selectorObject);

// Store the json formatted result in a var.
var selectionObject = JSON.parse(selectorObject.handleRememberSelection());
console.log("selectionObject:", selectionObject);

// Put the saved rememberme value of username in the username textbox.
$("#selectionBox select").val(selectionObject.selectedOption);
console.log("set select value to:", selectionObject.selectedOption);
// Put the saved rememberme value of password in the username textbox.

$("#saveSelectionSwitch input").prop("checked", selectionObject.checkState);

// Materialize replaces the native <select> with its own styled dropdown, but it only
// reads the selected <option> at init time. If nothing calls formSelect(), the visible
// dropdown never renders any text, appearing empty even though the underlying <select>
// has a value. Re-init (or init for the first time) here, after the value is set above,
// so the visible dropdown reflects the current selection.
console.log("initializing Materialize select (formSelect) so the dropdown UI reflects the selection");
$("#selectionBox select").formSelect();