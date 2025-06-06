import base64
import asyncio
import uvicorn
from fastapi import FastAPI, Request, BackgroundTasks
from pydantic import BaseModel
from typing import Optional

from nova_sonic import SimpleNovaSonic  # Import your class from another file

app = FastAPI()
nova_client = SimpleNovaSonic()


# Input model for audio chunk
class AudioChunk(BaseModel):
    base64Audio: str


@app.on_event("startup")
async def startup_event():
    # Initialize Nova Sonic client
    await nova_client.start_session()


@app.post("/start-session")
async def start_session():
    await nova_client.start_session()
    return {"status": "session started"}


@app.post("/start-audio")
async def start_audio():
    await nova_client.start_audio_input()
    return {"status": "audio input started"}


@app.post("/send-audio-chunk")
async def send_audio_chunk(chunk: AudioChunk):
    audio_bytes = base64.b64decode(chunk.base64Audio)
    await nova_client.send_audio_chunk(audio_bytes)
    return {"status": "chunk sent"}


@app.get("/get-audio-output")
async def get_audio_output():
    try:
        audio_bytes = await asyncio.wait_for(nova_client.audio_queue.get(), timeout=5)
        base64_audio = base64.b64encode(audio_bytes).decode()
        return {"audio": base64_audio}
    except asyncio.TimeoutError:
        return {"audio": None, "message": "No audio yet"}


@app.post("/end-session")
async def end_session():
    nova_client.is_active = False
    if nova_client.response and not nova_client.response.done():
        nova_client.response.cancel()
    await nova_client.end_session()
    return {"status": "session ended"}


@app.get("/health")
async def health_check():
    return {"status": "ok"}


if __name__ == "__main__":
    uvicorn.run("main:app", host="0.0.0.0", port=8000, reload=True)
