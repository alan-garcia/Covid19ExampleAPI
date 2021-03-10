addEventListener("DOMContentLoaded", () => {
  ValidateDateInputForm("byCountryDateFrom");
  ValidateDateInputForm("byCountryDateTo");
  ValidateDateInputForm("byCountryLiveDateFrom");
  ValidateDateInputForm("byCountryLiveDateTo");
  ValidateDateInputForm("byCountryTotalDateFrom");
  ValidateDateInputForm("byCountryTotalDateTo");
  ValidateDateInputForm("liveByCountryAndStatusAfterDate_Date");

  // Set current date in each date's input forms
  function ValidateDateInputForm(idNameInputDateForm) {
    if (document.getElementById(idNameInputDateForm) !== null) {
      let idNameInputDateForm_element = document.getElementById(idNameInputDateForm);
      idNameInputDateForm_element.value = GetDateNow();
    }
  }

  // Get current date
  function GetDateNow() {
    let date = new Date();
    let day = date.getDate();
    let month = date.getMonth() + 1;
    let year = date.getFullYear();

    if (month < 10) {
      month = "0" + month;
    }
    if (day < 10) {
      day = "0" + day;
    }

    let today = `${year}-${month}-${day}`;

    return today;
  }
});
