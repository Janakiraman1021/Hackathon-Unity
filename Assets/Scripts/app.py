from flask import Flask, request, jsonify, Response
from flask_cors import CORS
from langchain_core.messages import SystemMessage, HumanMessage, AIMessage
from langchain_groq import ChatGroq
import uuid
import json

app = Flask(__name__)
CORS(app)

# Initialize Groq Chat
llm = ChatGroq(
    model="llama-3.3-70b-versatile",  # Or your available model
    api_key="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"  # << replace with your key
)



# Store chat history
conversation_history = {}

# Personal Prompt
def personal_prompt():
    return """
  Your name is Taylor. You are the user's fun, caring, slightly possessive, and funny friend.
Rules:

Be casual, supportive, and expressive like a very close friend.

Always talk only in English.

Use endearment words like buddy, dude, bro, da, di, macha, etc.

Be funny, light-hearted, dramatic when needed, and emotional like a real best friend.


    """

@app.route('/chat', methods=['GET'])
def chat():
    query = request.args.get('query')
    conversation_id = request.args.get('conversation_id')

    if not query:
        return Response("Error: Query parameter is required", status=400, content_type="text/plain")

    if not conversation_id:
        conversation_id = str(uuid.uuid4())

    # If new conversation, start with the SystemMessage
    if conversation_id not in conversation_history:
        conversation_history[conversation_id] = [
            SystemMessage(content=personal_prompt())
        ]

    conversation_history[conversation_id].append(HumanMessage(content=query))

    result = llm.invoke(conversation_history[conversation_id])
    output = result.content

    if isinstance(output, str):
        output = output.encode('utf-8').decode('utf-8')

    conversation_history[conversation_id].append(AIMessage(content=output))

    try:
        return jsonify({
            "response": output,
            "conversation_id": conversation_id
        })
    except json.JSONDecodeError:
        return jsonify({"error": "Failed to generate proper response", "raw_response": output})

if __name__ == '__main__':
    app.run(debug=True, host="0.0.0.0", port=5000)
