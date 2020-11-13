Menu screens have two types of buttons:
- Navigation buttons
- Other buttons

Navigation buttons
Navigate between the menu screens, within the same menu
Require the "NavigationButton" component

Other buttons
These buttons can, for example, close the menu. The MenuManager component (that's assigned to the Menu prefab) has a function to close itself.
They can also send the game to a different scene. To do this, reference the GameManager prefab and use a "LoadScene" function. There are two functions: one uses a string to find the scene, the other uses a build index to find the scene. (Set in the Build Settings menu)

NOTE:
The volume slider in the options menu has not been implemented yet.