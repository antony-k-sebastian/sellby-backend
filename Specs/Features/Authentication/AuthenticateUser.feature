Feature: Login
    As a student 
    I want to log into Sellby 
    So that I can access the marketplace

    @api
    Scenario: Successfull OTP request via API
        Given the user provides a valid university email
        When they request a login OTP
        Then the OTP request should succeed

    @ui
    Scenario: Successfull login via UI
        Given the user is on login page
        When they enter a valid email and password
        And they click the login button
        Then they should be redirected to the marketplace homepage.


