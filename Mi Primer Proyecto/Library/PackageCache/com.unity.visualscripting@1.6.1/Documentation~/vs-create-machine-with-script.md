

# Creating a machine with a script graph

There are two types of graphs; script graphs and state graphs. The script graph is composed of decisions and flow, and is presented as a graph. The state graph has states defined by the user such as character running and jumping, and can have one script graph per state unit.

### To build a script machine graph

A Script Machine is a component that contains the visual representation of a script that is a script graph.

1. In the Scene, select an GameObject.
2. In the Inspector, click **Add Component**.
3. Select **Visual Scripting** > **Script Machine**.
   Unity adds a Script Machine component to the GameObject.
4. Select the source:
   - **Embed**: This is a graph that is contained directly inside the GameObject: the asset is saved to the GameObject, and not to a drive. </br>
     Note: Every modification you make in the Inspector during Play mode (runtime) is reset (lost) when you exit Play mode. For the edit to persist, you must make it outside Play mode.</br>
     Go to Step 6.
   - **Graph**: This is a normal graph. A Macro generates a new file that saves the asset on a drive and not to the GameObject.</br>
     Note: Use graphs to get scripts that are shareable and that you can edit in the Inspector during Play mode (runtime) without losing your modifications. Graphs are ideal for prototyping.
5. If the source is Graph:
   - For a new graph, create a new file. </br>
     Save the new graph (select **Save As**) and select a folder to save the file.v
     Click **Edit** **Graph** to see the new graph, which is automatically populated with both a Start and an Update event. To add other events, right-click on the graph and select **Add Event**.</br>
   - For an existing graph, in the Macro field, select the graph.</br>
     Unity loads the graph from the project and you can now edit it in the Inspector.
6. Use the fuzzy finder to add new units in the script graph.</br>
   When you drag a connector from a port onto a unit, the fuzzy finder provides a list of the legitimate nodes (nodes that can link to the source unit) that are most used. To find a specific unit, enter the name of the unit in the fuzzy finder search field. Link units with port connectors. 

