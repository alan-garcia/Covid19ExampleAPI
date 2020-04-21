$(function () {
  ValidateDateInputForm("byCountryDateFrom");
  ValidateDateInputForm("byCountryDateTo");
  ValidateDateInputForm("byCountryLiveDateFrom");
  ValidateDateInputForm("byCountryLiveDateTo");
  ValidateDateInputForm("byCountryTotalDateFrom");
  ValidateDateInputForm("byCountryTotalDateTo");
  ValidateDateInputForm("liveByCountryAndStatusAfterDate_Date");

  function ValidateDateInputForm(idNameInputDateForm) {
    if (document.getElementById(idNameInputDateForm) !== null) {
      var idNameInputDateForm_element = document.getElementById(idNameInputDateForm);
      idNameInputDateForm_element.value = GetDateNow();
    }
  }

  function GetDateNow() {
    var date = new Date();
    var day = date.getDate();
    var month = date.getMonth() + 1;
    var year = date.getFullYear();

    if (month < 10) month = "0" + month;
    if (day < 10) day = "0" + day;

    var today = year + "-" + month + "-" + day;

    return today;
  }
});