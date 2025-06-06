import os
from dotenv import load_dotenv
from azure.data.tables import TableServiceClient

load_dotenv()

# Azure Table Storage setup
connection_string = os.getenv("AZURE_TABLE_CONN_STRING")
print("Loaded connection string:", connection_string)

if not connection_string:
    raise ValueError("AZURE_TABLE_CONN_STRING is not set in the environment.")

table_name = "Transcripts"

# Create Table Service Client and Table Client
service_client = TableServiceClient.from_connection_string(conn_str=connection_string)
table_client = service_client.create_table_if_not_exists(table_name=table_name)

# Example save transcript function
def save_transcript(prompt_text, response_text):
    import uuid
    from datetime import datetime

    entity = {
        "PartitionKey": "Transcript",
        "RowKey": str(uuid.uuid4()),
        "PromptText": prompt_text,
        "ResponseText": response_text.strip(),
        "CreatedAt": datetime.utcnow().isoformat()
    }
    try:
        table_client.create_entity(entity=entity)
        print("Transcript saved successfully.")
    except Exception as e:
        print(f"Error saving transcript: {e}")

# Example usage
if __name__ == "__main__":
    save_transcript("Hello, how can I help you?", "You can help me with my order status.")
