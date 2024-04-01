# hello-ai

Uses Azure OpenAI (with an OpenAI LLM behind-the-scenes) to programmatically ask the AI to provide an interesting historical event. So far so good. Turns out this is easy for the AI.

The small twist is that it also asks that this event be one that "took place on this day in some past year" which turns out to really hard for the LLM because it has no idea what day it is today. But it still produces answers with grace and confidence! This aspect of responding with an answer that is not aligned with what was asked for in the promopt is often called a "halluciination" in the AI context.

The remedy is not very complex. The remedy is to use a very simple form of the RAG pattern to retrieve the date (the "R" in "RAG" is "retrieval") and augment the prompt with this information (the "A" is "augmentation"). 

Augmenting with relevant data that helps improve the accuracy of AI responses is known in AI circles as "grounding".

An instance of the application is running at <https://hello-ai.doingazure.com/>. You can conrol whether the AI is grounded (whether "today" is included in the prompt) via a checkbox toggle. You can then use the "Ask AI" button both with and without grounding and compare the outcomes. The rest of the web UI is read only (intensionally did not provide ability to send an arbitrary prompt).

The implementation is as an Azure Static Web App with a very simple web front end using plain old HTML, CSS, and JavaScrpt (no frameworks, in other words) and an Azure Function API written in C#.
