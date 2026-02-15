* goal
  * build a team productivity platform

# how has it been created?
* `specify init . --ai claude --script sh`
* | claude chat
  * `/speckit.constitution Taskify is a "Security-First" application. All user inputs must be validated. We use a microservices architecture. Code must be fully documented.`
    * check it's modified [.specify/memory/constitution.md](.specify/memory/constitution.md)
  * 

    ```bash
    /speckit.specify Develop Taskify, a team productivity platform
    * It should allow users to create projects, add team members, assign tasks, comment and move tasks between boards in Kanban style.
    * In this initial phase for this feature, let's call it "Create Taskify," let's have multiple users but the users will be declared ahead of time, predefined.
    * I want five users in two different categories, one product manager and four engineers. 
    * Let's create three different sample projects.
    * Let's have the standard Kanban columns for the status of each task, such as "To Do," "In Progress," "In Review," and "Done." 
    * There will be no login for this application as this is just the very first testing thing to ensure that our basic features are set up.
    ```
    * check that it   
      * created ["specs/"](../../../../specs)
      * branched | NEW branch
  * refine the spec
    * `/speckit.clarify I want to clarify the task card details. For each task in the UI for a task card, you should be able to change the current status of the task between the different columns in the Kanban work board. You should be able to leave an unlimited number of comments for a particular card. You should be able to, from that task card, assign one of the valid users.`
    * `/speckit.clarify When you first launch Taskify, it's going to give you a list of the five users to pick from. There will be no password required. When you click on a user, you go into the main view, which displays the list of projects. When you click on a project, you open the Kanban board for that project. You're going to see the columns. You'll be able to drag and drop cards back and forth between different columns. You will see any cards that are assigned to you, the currently logged in user, in a different color from all the other ones, so you can quickly see yours. You can edit any comments that you make, but you can't edit comments that other people made. You can delete any comments that you made, but you can't delete comments anybody else made.`
  * `/speckit.checklist`
  * 
    ```bash
    /speckit.plan We are going to generate this using .NET Aspire, using Postgres as the database.
    * The frontend should use Blazor server with drag-and-drop task boards, real-time updates.
    * There should be a REST API created with a projects API, tasks API, and a notifications API.
    ```
    * create [specs/*/research.md](specs/001-photo-album-organizer/research.md)
    * modify [specs/*/plan.md](specs/001-photo-album-organizer/plan.md)
  * `/speckit.tasks`
    * create [specs/*/tasks.md](specs/001-photo-album-organizer/tasks.md)
  * `/speckit.analyze`
  * `/speckit.implement`
