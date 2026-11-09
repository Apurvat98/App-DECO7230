# Immersive Chemistry Lab

**A virtual-reality chemistry experience built with Unity for Meta Quest.**

Immersive Chemistry Lab is an interactive XR prototype developed for **DECO7230 - Digital Prototyping and Extended Reality**, Semester 2, 2026, at The University of Queensland. It combines hands-on laboratory activities with a transition into a molecular-scale environment, where users can construct and manipulate a simple water molecule.

The prototype uses an alchemy-inspired laboratory setting. Its main design goal is to explore whether spatial instructions, controller-based interactions, and visual feedback make the activities understandable and comfortable to use. It is a **prototype**, not a validated chemistry-teaching tool.

## Experience overview

The experience contains three laboratory activities and a molecular exploration sequence:

1. **Combining reactants:** Pick up two bottles, pour their contents into a cauldron, and observe the reaction response.
2. **Molecular exploration:** Trace the circular portal with a controller to enter a molecular-scale space. Grab hydrogen atoms, connect them to oxygen to form H₂O, and pull atoms apart to explore bond manipulation.
3. **Magical mixing:** Pick up a wooden spoon and stir the mixture in the bowl to trigger a visible change.
4. **Heating reaction:** Hold the flask over the burner and gently rotate it to activate the heating response.

The laboratory includes instructional boards, station guidance, and visual feedback to help users progress between activities.

## Main VR controls

| Action | Controls or gesture |
| --- | --- |
| Navigate around the laboratory | Controller thumbstick |
| Grab and move an interactive object | Controller Grip |
| Pour a reactant | Grab and tilt the bottle over the cauldron |
| Enter the molecular world | Trace the indicated circular portal with a controller |
| Form or break a molecular bond | Bring atoms together or pull connected atoms apart |
| Return to the laboratory | **B** button |
| Stir the mixture | Grab the spoon and move it in circles inside the bowl |
| Activate the heating experiment | Hold and gently rotate the flask over the burner |

*Controls are based on the final tested prototype. Controller visual alignment and device compatibility should be checked on the headset used for a demonstration.*

## Development and technology

- **Engine:** Unity 6
- **Target hardware:** Meta Quest VR headset with tracked controllers
- **XR framework:** OpenXR and XR Interaction Toolkit
- **Scripting:** C#
- **Testing:** Physical-headset moderated usability sessions; Unity XR simulator used during earlier iterations

The project evolved through three stages:

- **IP1:** Initial reaction, molecular transition, and atom/bond manipulation interactions.
- **IP2a:** Improvements to controller flexibility, proximity cues, and molecular instructions and feedback.
- **IP2b:** Expanded laboratory with circular portal tracing, stirring and heating experiments, station guidance, and a full Meta Quest testing session.

## Opening and running the project

1. Clone or download this repository and open the Unity project through **Unity Hub** using the compatible Unity 6 editor.
2. Allow Unity to import the project and resolve the package dependencies listed in `Packages/manifest.json`.
3. Open the final Quest scene, **`Final_ImmersiveChemistryLab_Quest`**, in the Unity Editor.
4. For headset testing, configure the **Android** build target and check **OpenXR**, Meta Quest support, and the controller interaction profile appropriate to the connected headset.
5. Ensure the **XR Interaction Simulator** is disabled for the device build, then use **Build and Run** with a connected Meta Quest headset in developer mode.
6. Check headset tracking, controller orientation, Grip interaction, thumbstick navigation, and the starting position before testing.

> A desktop simulator can help with development, but it is not a substitute for physical-headset testing. An Android build may also require a compatible Unity installation and Android build support modules.

## User testing and evaluation

The final IP2b prototype was evaluated with **four participants** through moderated, task-based think-aloud sessions on a Meta Quest headset. Participants were asked to follow the laboratory instructions, complete the interaction sequence, and comment on usability and comfort. Facilitator assistance was sometimes provided, so completing an activity should **not** be interpreted as independent success.

The main observations were:

- All four participants reached the main activities and assembled H₂O during the moderated sessions.
- Participants 2 and 4 had to retry the portal gesture after an initial unsuccessful attempt, suggesting a need for clearer progress and retry feedback.
- Direct atom manipulation was a positive aspect of the experience.
- Participants generally understood the actions required, but requested clearer explanations of the chemistry represented by the visual effects.
- No participant reported dizziness or discomfort during the recorded sessions.

The final evaluation therefore **partially supported** the concept's interaction usability. Chemistry learning outcomes were **not formally measured**.

## Assessment documents

The associated materials prepared for the coursework include:

- **IP2b testing plan**, including the postgraduate research summary
- **Design Evaluation 3 and Final Reflection**, including synthesised results and participant evidence
- **AI acknowledgement coversheet**, submitted where required by the course

These documents can be added under a `Documentation/` directory in this repository. Refer to the files actually committed to the repository; this README does not imply that any document has already been uploaded.

## Third-party asset attribution

The laboratory environment incorporates **Alchemist House**, a 3D environment asset pack by **BK**, obtained through the Unity Asset Store:

[Alchemist House – Unity Asset Store](https://assetstore.unity.com/packages/3d/environments/alchemist-house-112442)

The asset pack supplied environmental models and decorative elements used to construct the setting. The chemistry-specific interaction logic, molecular activities, portal mechanics, instructions, and related prototype work were developed and customised for this coursework, with AI assistance disclosed separately in the assessment acknowledgement.

**Asset licensing:** Third-party Unity Asset Store content remains subject to its original licence. Do not assume the environmental assets can be redistributed independently or included in a public repository without checking the applicable licence. This repository's student-project documentation does not transfer ownership of those assets.

## Known limitations and next steps

- Portal tracing needs more immediate feedback on gesture recognition and failed attempts.
- Station completion and the return control could be made more discoverable.
- The experiments need stronger explanations of the substances and chemical processes they represent.
- The molecular H₂O interaction is a simplified learning visualisation; it should not be presented as a complete or verified chemical model of the preceding reaction.
- A broader, more consistently scored headset study would be needed to evaluate independent usability and actual learning outcomes.

**Project:** Immersive Chemistry Lab  
**Course:** DECO7230 – Digital Prototyping and Extended Reality  
**Institution:** The University of Queensland  
**Semester:** 2, 2026
