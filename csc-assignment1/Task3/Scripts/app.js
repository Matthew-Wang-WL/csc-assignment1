﻿function ViewModel() {
    var self = this;

    var tokenKey = 'accessToken';

    self.result = ko.observable();
    self.user = ko.observable();

    self.registerEmail = ko.observable();
    self.registerPassword = ko.observable();
    self.registerPassword2 = ko.observable();

    self.loginEmail = ko.observable();
    self.loginPassword = ko.observable();
    self.errors = ko.observableArray([]);

    function showError(jqXHR) {

        self.result(jqXHR.status + ': ' + jqXHR.statusText);

        var response = jqXHR.responseJSON;
        if (response) {
            if (response.Message) self.errors.push(response.Message);
            if (response.ModelState) {
                var modelState = response.ModelState;
                for (var prop in modelState)
                {
                    if (modelState.hasOwnProperty(prop)) {
                        var msgArr = modelState[prop]; // expect array here
                        if (msgArr.length) {
                            for (var i = 0; i < msgArr.length; ++i) self.errors.push(msgArr[i]);
                        }
                    }
                }
            }
            if (response.error) self.errors.push(response.error);
            if (response.error_description) self.errors.push(response.error_description);
        }
    }

    self.callApi = function () {
        self.result('');
        self.errors.removeAll();

        grecaptcha.ready(function () {
            grecaptcha.execute('6Lffcf4UAAAAADn6po56Kg9DKhTCq-yrc8X61O_b', { action: 'submit' }).then(function (captchaToken) {
                var token = sessionStorage.getItem(tokenKey);
                var headers = {};
                if (token) {
                    headers.Authorization = 'Bearer ' + token;
                }
                var captchaToken = {
                    UserToken: captchaToken
                }

                // Validate the token
                $.ajax({
                    type: 'POST',
                    url: '/api/Account/ValidateToken',
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify(captchaToken)
                }).done(function (response) {
                    //Token validated, check if successful
                    var isValidToken = response.success;
                    if (isValidToken) {
                        $.ajax({
                            type: 'GET',
                            url: '/api/values',
                            headers: headers
                        }).done(function (data) {
                            self.result(data);
                        }).fail(showError);
                    } else {
                        self.result("Fail!")
                        self.errors.push("ReCAPTCHA is invalid or has expired.");
                    }
                }).fail(showError);
            });
        });
    }

    self.register = function () {
        self.result('');
        self.errors.removeAll();

        grecaptcha.ready(function () {
            grecaptcha.execute('6Lffcf4UAAAAADn6po56Kg9DKhTCq-yrc8X61O_b', { action: 'submit' }).then(function (token) {
                var data = {
                    Email: self.registerEmail(),
                    Password: self.registerPassword(),
                    ConfirmPassword: self.registerPassword2(),
                };
                var token = {
                    UserToken: token
                }

                // Validate the token
                $.ajax({
                    type: 'POST',
                    url: '/api/Account/ValidateToken',
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify(token)
                }).done(function (response) {
                    //Token validated, check if successful
                    var isValidToken = response.success;
                    if (isValidToken) {
                        $.ajax({
                            type: 'POST',
                            url: '/api/Account/Register',
                            contentType: 'application/json; charset=utf-8',
                            data: JSON.stringify(data)
                        }).done(function (data) {
                            self.result("Done!");
                        }).fail(showError);
                    } else {
                        self.result("Fail!")
                        self.errors.push("ReCAPTCHA is invalid or has expired.");
                    }
                }).fail(showError);
                
            });
        });
    }

    self.login = function () {
        self.result('');
        self.errors.removeAll();

        grecaptcha.ready(function () {
            grecaptcha.execute('6Lffcf4UAAAAADn6po56Kg9DKhTCq-yrc8X61O_b', { action: 'submit' }).then(function (token) {
                var loginData = {
                    grant_type: 'password',
                    username: self.loginEmail(),
                    password: self.loginPassword()
                };
                var token = {
                    UserToken: token
                }

                // Validate the token
                $.ajax({
                    type: 'POST',
                    url: '/api/Account/ValidateToken',
                    contentType: 'application/json; charset=utf-8',
                    data: JSON.stringify(token)
                }).done(function (response) {
                    //Token validated, check if successful
                    var isValidToken = response.success;
                    if (isValidToken) {
                        $.ajax({
                            type: 'POST',
                            url: '/Token',
                            data: loginData
                        }).done(function (data) {
                            self.user(data.userName);
                            // Cache the access token in session storage.
                            sessionStorage.setItem(tokenKey, data.access_token);
                        }).fail(showError);
                    } else {
                        self.result("Fail!")
                        self.errors.push("ReCAPTCHA is invalid or has expired.");
                    }
                }).fail(showError);
            });
        });
    }

    self.logout = function () {
        // Log out from the cookie based logon.
        var token = sessionStorage.getItem(tokenKey);
        var headers = {};
        if (token) {
            headers.Authorization = 'Bearer ' + token;
        }

        $.ajax({
            type: 'POST',
            url: '/api/Account/Logout',
            headers: headers
        }).done(function (data) {
            // Successfully logged out. Delete the token.
            self.user('');
            sessionStorage.removeItem(tokenKey);
        }).fail(showError);
    }
}

var app = new ViewModel();
ko.applyBindings(app);