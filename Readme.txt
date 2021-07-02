Here's the GEDCOM to CK3 conversion tool, version 1.0. This thing facilitates character history file creation by allowing you to use software actually designed for family tree creation, such as FamilyEcho or Simple Family Tree. Such software usually exports to the GEDCOM format, a standard for genealogy. Assuming that the GEDCOM file is properly formatted, my tool take that as input, and produce the required files to make the same family show up in the game.

Input:
 - A GEDCOM file, with the extension .ged.
	If you're not sure how to make one, consider https://www.familyecho.com/. However, any self-respecting genealogy software should work, so you should pick whichever you prefer.
	CK3 specific notes:
		- You can use the description/notes field to add extra information about the character. You can add keys for faiths, cultures, and traits (e.g. baltic_pagan, education_stewardship_3). They will be applied to the character. Though for faith/culture it is more convenient to use the dynasty or house definition instead.
		- Make sure to write dates in the standard GEDCOM format ("6 APR 2001"), or with just the year ("2001"). The tool will use the first of January in that case.
		- If a character is married, make sure that marriage has a specified wedding date.
		- A character without a death date will have one generated randomly.

 - A dynasties file called Dynasties.csv, and a houses file called Houses.csv.
	Here you should define all the dynasties and dynasty houses your mod is going to use. Those can be vanilla dynasties/houses too, in which case you can mark them so in the file. See file itself for explanation about the required fields.
	
	You can also define a culture/faith here, which will be applied to every member of the house/dynasty. That spares you the effort of marking every person with the same culture/faith. However, any culture/faith given in a person's description will override this.

	Note that characters are matched to dynasties/houses based on their COMBINED surname. A house with a prefix will have that prefix appended to the name before such matching takes place. That means that a character named Karl /von Habsburg/ will match with a dynasty with name "Habsburg" and prefix "von " (note the space) as specified in Dynasties.csv.
	If both a dynasty and dynasty_house exist with the same combined name, the house takes precedence. If none are found, the character is considered lowborn.

	The dynasties you define need id strings. Those can be plain numbers, namespaced numbers ("mymod.1") or just words ("badass_dynasty_x").

 - Vanilla reference files; Cultures.txt, Faiths.txt, Traits.txt.
	These are referenced when looking at a person's description for traits/culture/faith. You can add or change these if you are generating for a mod.
 - A settings file called Settings.txt
	More ways to tweak the functionality of this tool.

Output:
 - Character file(s); grouped together according to your setting.
 - Dynasty and house files called generated_dynasties.txt and generated_dynasty_houses.txt. Dynasties/houses marked as IsVanilla will not be generated here.
 - Localisation called generated_loc_l_<LANGUAGE>.yml. This will be localisation for the names of your houses/dynasties, and any prefixes. The latter will probably duplicate vanilla quite often, but I have no way to tell which.

Cache:
 - A cache file, named after the corresponding GEDCOM file, is also generated upon running this tool, and is read when you run it again. This will try to match the same GEDCOM people with the same (generated) CK3 ids over multiple times you run the tool, even if you add or delete people.


Did that make sense? If you have any questions, check the forum thread here: https://forum.paradoxplaza.com/forum/threads/tool-gedcom-to-ck3-converter.1481724/