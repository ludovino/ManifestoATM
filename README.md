# ManifestoATM
Technical test for Manifesto

time taken: ~4 hours

## how to test

Clone this repository to your local machine. Open the solution in visual studio and run the tests.

In the build dropdown hit publish, start, and select publish to folder. Press publish, then run the Runner.bat file in the output folder.

OR

debug should work fine


## decisions I've made

I thought using an inversion of control framework would be a bit over the top for a console app so I just use concrete classes and inject them.

I split the responsibilities of parsing the instructions and processing them. The Atm class handles the session and output rules, and the AtmCommandReader handles the parsing of input text.

The command reader will just throw an InvalidOperationException if the input is malformed. A malformed input string would not be something that a user can fix, so I think throwing an exception is appropriate. To save time I have not unit tested this behavior.

I am treating the Session class as an implementation detail of the Atm object, so no unit tests for it. Maybe should have made it a nested class.