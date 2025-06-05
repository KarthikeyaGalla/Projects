"use client"

import { useState, useEffect, useRef } from "react"
import axios from "axios"
import { v4 as uuidv4 } from "uuid"
import { CodeBlock } from "./components/code-block"

export default function App() {
  const [prompt, setPrompt] = useState("")
  const [fileName, setFileName] = useState("")
  const [messages, setMessages] = useState([])
  const [history, setHistory] = useState([])
  const [selectedFile, setSelectedFile] = useState(null)
  const [isUploading, setIsUploading] = useState(false)
  const [isSending, setIsSending] = useState(false)
  const [showSidebar, setShowSidebar] = useState(true)
  const [isTyping, setIsTyping] = useState(false)
  const [theme, setTheme] = useState("light") // light or dark
  const messagesEndRef = useRef(null)
  const chatContainerRef = useRef(null)
  const [showWelcomeAnimation, setShowWelcomeAnimation] = useState(true)

  // Fetch chat history on load
  useEffect(() => {
    fetchHistory()

    // Hide welcome animation after 3 seconds
    const timer = setTimeout(() => {
      setShowWelcomeAnimation(false)
    }, 3000)

    return () => clearTimeout(timer)
  }, [])

  // Auto-scroll to bottom when messages change
  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: "smooth" })
  }, [messages])

  // Add particle background effect
  useEffect(() => {
    const createParticles = () => {
      if (!chatContainerRef.current) return

      const container = chatContainerRef.current
      const particleCount = 50

      // Clear any existing particles
      const existingParticles = container.querySelectorAll(".particle")
      existingParticles.forEach((p) => p.remove())

      for (let i = 0; i < particleCount; i++) {
        const particle = document.createElement("div")
        particle.className = "particle absolute rounded-full pointer-events-none opacity-0"

        // Random size between 2px and 6px
        const size = Math.random() * 4 + 2
        particle.style.width = `${size}px`
        particle.style.height = `${size}px`

        // Random position
        particle.style.left = `${Math.random() * 100}%`
        particle.style.top = `${Math.random() * 100}%`

        // Set color based on theme
        particle.style.backgroundColor = theme === "light" ? "rgba(59, 130, 246, 0.3)" : "rgba(219, 234, 254, 0.3)"

        // Animation duration between 10s and 30s
        const duration = Math.random() * 20 + 10
        particle.style.animation = `float ${duration}s linear infinite`

        // Random delay so they don't all move together
        particle.style.animationDelay = `${Math.random() * duration}s`

        container.appendChild(particle)

        // Fade in
        setTimeout(() => {
          particle.style.opacity = "1"
          particle.style.transition = "opacity 1s ease-in-out"
        }, 100)
      }
    }

    createParticles()

    // Recreate particles when theme changes
    window.addEventListener("resize", createParticles)

    return () => {
      window.removeEventListener("resize", createParticles)
    }
  }, [theme])

  const fetchHistory = async () => {
    try {
      const response = await axios.get("http://localhost:5108/api/Gemini/RetriveData")
      const sortedData = response.data.sort((a, b) => new Date(b.timestamp) - new Date(a.timestamp))
      setHistory(sortedData)
    } catch (error) {
      console.error("Error fetching history:", error)
    }
  }

  // Function to parse code blocks from text
  const parseCodeBlocks = (text) => {
    // Regex to match code blocks with language and optional explanation
    const codeBlockRegex = /```(\w+)([\s\S]*?)```\s*(?:\*\*Explanation:\*\*([\s\S]*?)(?=```|\*\*|$))?/g

    let lastIndex = 0
    const parts = []
    let match

    while ((match = codeBlockRegex.exec(text)) !== null) {
      // Add text before the code block
      if (match.index > lastIndex) {
        parts.push({
          type: "text",
          content: text.substring(lastIndex, match.index),
        })
      }

      // Add the code block
      parts.push({
        type: "code",
        language: match[1],
        code: match[2].trim(),
        explanation: match[3] ? match[3].trim() : null,
      })

      lastIndex = match.index + match[0].length
    }

    // Add any remaining text
    if (lastIndex < text.length) {
      parts.push({
        type: "text",
        content: text.substring(lastIndex),
      })
    }

    return parts
  }

  const simulateTyping = (text, callback) => {
    setIsTyping(true)

    // Create a temporary message for typing animation
    const typingMsgId = uuidv4()
    const typingMsg = { id: typingMsgId, sender: "bot", text: "", isTyping: true, parts: [] }
    setMessages((prev) => [...prev, typingMsg])

    let charIndex = 0
    const typingSpeed = 10 // ms per character

    const typeChar = () => {
      if (charIndex < text.length) {
        setMessages((prev) =>
          prev.map((msg) => (msg.id === typingMsgId ? { ...msg, text: text.substring(0, charIndex + 1) } : msg)),
        )
        charIndex++
        setTimeout(typeChar, typingSpeed)
      } else {
        setIsTyping(false)
        // Parse code blocks after typing is complete
        const parts = parseCodeBlocks(text)
        setMessages((prev) => prev.map((msg) => (msg.id === typingMsgId ? { ...msg, isTyping: false, parts } : msg)))
        if (callback) callback()
      }
    }

    typeChar()
    return typingMsgId
  }

  const handleSend = async () => {
    if (!prompt.trim()) return

    // Add user message with animation
    const userMessage = {
      id: uuidv4(),
      sender: "user",
      text: prompt,
      new: true,
      parts: [{ type: "text", content: prompt }],
    }
    setMessages((prev) => [...prev, userMessage])
    setIsSending(true)

    // Remove 'new' flag after animation completes
    setTimeout(() => {
      setMessages((prev) => prev.map((msg) => (msg.id === userMessage.id ? { ...msg, new: false } : msg)))
    }, 500)

    try {
      let response
      let responseText

      // Choose API endpoint based on whether a filename is provided
      if (fileName.trim()) {
        // If filename is provided, use the read-content API
        response = await axios.post("http://localhost:5108/api/FilesHandle/read-content", {
          fileName: fileName,
          prompt,
        })
        responseText = response.data.result

        // Add a system message about the file being used
        const systemMessage = {
          id: uuidv4(),
          sender: "system",
          text: `Using file: "${fileName}"`,
          new: true,
          parts: [{ type: "text", content: `Using file: "${fileName}"` }],
        }
        setMessages((prev) => [...prev, systemMessage])

        // Remove 'new' flag after animation completes
        setTimeout(() => {
          setMessages((prev) => prev.map((msg) => (msg.id === systemMessage.id ? { ...msg, new: false } : msg)))
        }, 500)
      } else {
        // If no filename, use the regular generate API
        response = await axios.post("http://localhost:5108/api/Gemini/generate", {
          prompt,
        })
        responseText = response.data.content
      }

      // Use typing animation for bot response
      simulateTyping(responseText, null)
    } catch (error) {
      console.error("Error sending prompt:", error)
      const errorMessage = {
        id: uuidv4(),
        sender: "bot",
        text: "Something went wrong. Please try again.",
        parts: [{ type: "text", content: "Something went wrong. Please try again." }],
      }
      setMessages((prev) => [...prev, errorMessage])
    } finally {
      setIsSending(false)
      setPrompt("")
    }
  }

  const handleKeyDown = (e) => {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault()
      handleSend()
    }
  }

  const handleFileUpload = async () => {
    if (!selectedFile) return

    const formData = new FormData()
    formData.append("file", selectedFile)
    setIsUploading(true)

    try {
      await axios.post("http://localhost:5108/api/FilesHandle/upload", formData, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      })

      // Add a system message about successful upload
      const systemMessage = {
        id: uuidv4(),
        sender: "system",
        text: `File "${selectedFile.name}" uploaded successfully!`,
        new: true,
        parts: [{ type: "text", content: `File "${selectedFile.name}" uploaded successfully!` }],
      }
      setMessages((prev) => [...prev, systemMessage])

      // Remove 'new' flag after animation completes
      setTimeout(() => {
        setMessages((prev) => prev.map((msg) => (msg.id === systemMessage.id ? { ...msg, new: false } : msg)))
      }, 500)

      setSelectedFile(null)
    } catch (error) {
      console.error("Error uploading file:", error)
      const errorMessage = {
        id: uuidv4(),
        sender: "system",
        text: "Failed to upload file. Please try again.",
        new: true,
        parts: [{ type: "text", content: "Failed to upload file. Please try again." }],
      }
      setMessages((prev) => [...prev, errorMessage])

      // Remove 'new' flag after animation completes
      setTimeout(() => {
        setMessages((prev) => prev.map((msg) => (msg.id === errorMessage.id ? { ...msg, new: false } : msg)))
      }, 500)
    } finally {
      setIsUploading(false)
    }
  }

  const formatDate = (dateString) => {
    const date = new Date(dateString)
    return date.toLocaleString()
  }

  const toggleTheme = () => {
    setTheme(theme === "light" ? "dark" : "light")
  }

  return (
    <div
      className={`flex h-screen transition-colors duration-500 ${
        theme === "light"
          ? "bg-gradient-to-br from-white to-blue-50 text-gray-800"
          : "bg-gradient-to-br from-gray-900 to-blue-900 text-white"
      }`}
    >
      {/* Welcome Animation */}
      {showWelcomeAnimation && (
        <div className="fixed inset-0 flex items-center justify-center z-50 bg-gradient-to-r from-blue-600 to-indigo-600">
          <div className="text-center">
            <div className="text-5xl font-bold text-white mb-4 animate-pulse">Sonata Assistant</div>
            <div className="relative w-64 h-2 bg-blue-300 rounded-full mx-auto overflow-hidden">
              <div className="absolute top-0 left-0 h-full bg-white animate-[loading_2s_ease-in-out_infinite]"></div>
            </div>
          </div>
        </div>
      )}

      {/* Sidebar Toggle Button (Mobile) */}
      <button
        className="md:hidden fixed top-4 left-4 z-50 bg-blue-600 text-white p-2 rounded-full shadow-lg hover:bg-blue-700 transition-all duration-300 transform hover:scale-110"
        onClick={() => setShowSidebar(!showSidebar)}
      >
        {showSidebar ? (
          <svg
            xmlns="http://www.w3.org/2000/svg"
            className="h-6 w-6"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
          </svg>
        ) : (
          <svg
            xmlns="http://www.w3.org/2000/svg"
            className="h-6 w-6"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 6h16M4 12h16M4 18h16" />
          </svg>
        )}
      </button>

      {/* Theme Toggle Button */}
      <button
        className="fixed top-4 right-4 z-50 p-2 rounded-full shadow-lg transition-all duration-300 transform hover:scale-110"
        onClick={toggleTheme}
        style={{
          background:
            theme === "light"
              ? "linear-gradient(135deg, #1e3a8a 0%, #3b82f6 100%)"
              : "linear-gradient(135deg, #dbeafe 0%, #93c5fd 100%)",
        }}
      >
        {theme === "light" ? (
          <svg
            xmlns="http://www.w3.org/2000/svg"
            className="h-6 w-6 text-white"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M20.354 15.354A9 9 0 018.646 3.646 9.003 9.003 0 0012 21a9.003 9.003 0 008.354-5.646z"
            />
          </svg>
        ) : (
          <svg
            xmlns="http://www.w3.org/2000/svg"
            className="h-6 w-6 text-blue-900"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M12 3v1m0 16v1m9-9h-1M4 12H3m15.364 6.364l-.707-.707M6.343 6.343l-.707-.707m12.728 0l-.707.707M6.343 17.657l-.707.707M16 12a4 4 0 11-8 0 4 4 0 018 0z"
            />
          </svg>
        )}
      </button>

      {/* Sidebar for History */}
      <div
        className={`${showSidebar ? "translate-x-0" : "-translate-x-full"} 
                      md:translate-x-0 transition-all duration-500 ease-in-out
                      w-full md:w-80 lg:w-96 
                      fixed md:relative h-full z-40 overflow-y-auto
                      ${
                        theme === "light"
                          ? "bg-gradient-to-b from-blue-50 to-white border-r border-blue-100"
                          : "bg-gradient-to-b from-gray-900 to-blue-900 border-r border-blue-800"
                      }
                      backdrop-blur-md bg-opacity-90`}
        style={{
          boxShadow: theme === "light" ? "0 4px 20px rgba(59, 130, 246, 0.1)" : "0 4px 20px rgba(30, 58, 138, 0.3)",
        }}
      >
        <div
          className={`p-6 border-b ${theme === "light" ? "border-blue-100" : "border-blue-800"} flex items-center space-x-3`}
        >
          <div className="relative">
            <img
              src="/robot.png"
              alt="Logo"
              // className="h-10 w-10 rounded-full object-cover animate-pulse"
              className="h-15 w-20"
            />
            <div className="absolute inset-0 rounded-full "></div>
          </div>
          <h2 className={`text-2xl font-bold ${theme === "light" ? "text-blue-700" : "text-blue-300"}`}>
            Chat History
          </h2>
        </div>

        <div className="p-4 space-y-4">
          {history.length === 0 ? (
            <div className={`text-center ${theme === "light" ? "text-gray-500" : "text-gray-400"} py-8 animate-pulse`}>
              No chat history yet
            </div>
          ) : (
            history.map((item, index) => (
              <div
                key={item.rowKey}
                className={`${theme === "light" ? "bg-white hover:bg-blue-50" : "bg-blue-900/30 hover:bg-blue-800/50"} 
                  rounded-xl p-5 shadow-sm hover:shadow-md transition-all duration-300 transform hover:-translate-y-1
                  animate-[fadeIn_0.5s_ease-in-out]`}
                style={{
                  animationDelay: `${index * 0.1}s`,
                  backdropFilter: "blur(10px)",
                }}
              >
                <div className={`text-xs ${theme === "light" ? "text-gray-500" : "text-gray-400"} mb-2`}>
                  {formatDate(item.timestamp)}
                </div>
                <div className="mb-2">
                  <span className={`${theme === "light" ? "text-blue-600" : "text-blue-300"} font-medium`}>
                    Prompt:
                  </span>
                  <span className={`${theme === "light" ? "text-gray-700" : "text-gray-300"}`}>{item.request}</span>
                </div>
                <div>
                  <span className={`${theme === "light" ? "text-blue-600" : "text-blue-300"} font-medium`}>
                    Response:
                  </span>
                  <span className={`${theme === "light" ? "text-gray-700" : "text-gray-300"} text-sm line-clamp-3`}>
                    {item.response}
                  </span>
                </div>
              </div>
            ))
          )}
        </div>
      </div>

      {/* Main Chat Area */}
      <div className="flex-1 flex flex-col relative" ref={chatContainerRef}>
        {/* Particle background */}
        <style jsx>{`
          @keyframes float {
            0% {
              transform: translateY(0) translateX(0);
            }
            25% {
              transform: translateY(-20px) translateX(10px);
            }
            50% {
              transform: translateY(0) translateX(20px);
            }
            75% {
              transform: translateY(20px) translateX(10px);
            }
            100% {
              transform: translateY(0) translateX(0);
            }
          }
          
          @keyframes fadeIn {
            from { opacity: 0; transform: translateY(20px); }
            to { opacity: 1; transform: translateY(0); }
          }
          
          @keyframes loading {
            0% { width: 0%; left: 0; }
            50% { width: 100%; left: 0; }
            100% { width: 0%; left: 100%; }
          }
          
          @keyframes pulse-border {
            0% { box-shadow: 0 0 0 0 rgba(59, 130, 246, 0.7); }
            70% { box-shadow: 0 0 0 10px rgba(59, 130, 246, 0); }
            100% { box-shadow: 0 0 0 0 rgba(59, 130, 246, 0); }
          }
          
          @keyframes typing {
            from { width: 0 }
            to { width: 100% }
          }
          
          @keyframes blink {
            50% { border-color: transparent }
          }
        `}</style>

        {/* Chat Header */}
        <div
          className={`${
            theme === "light" ? "bg-white/80 border-b border-blue-100" : "bg-blue-900/30 border-b border-blue-800"
          } 
            backdrop-blur-md p-6 flex items-center justify-center shadow-sm`}
        >
          <h1
            className={`text-2xl font-bold ${theme === "light" ? "text-blue-700" : "text-blue-300"} flex items-center`}
          >
            <span className="relative">
              <span className="absolute -left-8 top-1/2 transform -translate-y-1/2 w-6 h-6 rounded-full bg-gradient-to-r from-blue-400 to-blue-600 animate-pulse"></span>
            </span>
            Sonata Assistant
            <span className="ml-2 text-xs px-2 py-1 rounded-full bg-gradient-to-r from-blue-500 to-indigo-500 text-white">
              v2.0
            </span>
          </h1>
        </div>

        {/* Messages Area */}
        <div className={`flex-1 p-4 md:p-6 overflow-y-auto ${theme === "light" ? "bg-transparent" : "bg-transparent"}`}>
          {messages.length === 0 ? (
            <div className="h-full flex flex-col items-center justify-center text-center p-8 animate-[fadeIn_1s_ease-in-out]">
              <div className="w-24 h-24 relative mb-6">
                <div className="absolute inset-0 bg-gradient-to-r from-blue-400 to-blue-600 rounded-full animate-pulse"></div>
                <div className="absolute inset-2 bg-gradient-to-r from-blue-100 to-blue-300 rounded-full flex items-center justify-center">
                  <svg
                    xmlns="http://www.w3.org/2000/svg"
                    className={`h-12 w-12 ${theme === "light" ? "text-blue-600" : "text-blue-800"}`}
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor"
                  >
                    <path
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      strokeWidth={2}
                      d="M8 10h.01M12 10h.01M16 10h.01M9 16H5a2 2 0 01-2-2V6a2 2 0 012-2h14a2 2 0 012 2v8a2 2 0 01-2 2h-5l-5 5v-5z"
                    />
                  </svg>
                </div>
              </div>
              <h2 className={`text-2xl font-bold ${theme === "light" ? "text-gray-700" : "text-gray-200"} mb-4`}>
                How can I help you today?
              </h2>
              <p className={`${theme === "light" ? "text-gray-500" : "text-gray-400"} max-w-md`}>
                Ask me anything or upload a file to get started.
              </p>

              <div className="mt-8 flex flex-wrap justify-center gap-4">
                {["Tell me a joke", "What's the weather like?", "Help me with coding"].map((suggestion, i) => (
                  <button
                    key={i}
                    className={`px-4 py-2 rounded-full text-sm ${
                      theme === "light"
                        ? "bg-blue-100 text-blue-700 hover:bg-blue-200"
                        : "bg-blue-800/50 text-blue-200 hover:bg-blue-700/50"
                    } transition-all duration-300 transform hover:scale-105`}
                    onClick={() => {
                      setPrompt(suggestion)
                      setTimeout(() => handleSend(), 100)
                    }}
                  >
                    {suggestion}
                  </button>
                ))}
              </div>
            </div>
          ) : (
            <div className="space-y-6 max-w-3xl mx-auto">
              {messages.map((msg) => (
                <div
                  key={msg.id}
                  className={`flex ${msg.sender === "user" ? "justify-end" : "justify-start"} ${
                    msg.new ? "animate-[fadeIn_0.5s_ease-in-out]" : ""
                  }`}
                >
                  {msg.sender !== "user" && (
                    <div className="w-8 h-8 rounded-full bg-gradient-to-r from-blue-400 to-blue-600 flex items-center justify-center mr-2 flex-shrink-0 shadow-lg">
                      <svg
                        xmlns="http://www.w3.org/2000/svg"
                        className="h-5 w-5 text-white"
                        fill="none"
                        viewBox="0 0 24 24"
                        stroke="currentColor"
                      >
                        <path
                          strokeLinecap="round"
                          strokeLinejoin="round"
                          strokeWidth={2}
                          d="M9 3v2m6-2v2M9 19v2m6-2v2M5 9H3m2 6H3m18-6h-2m2 6h-2M7 19h10a2 2 0 002-2V7a2 2 0 00-2-2H7a2 2 0 00-2 2v10a2 2 0 002 2zM9 9h6v6H9V9z"
                        />
                      </svg>
                    </div>
                  )}

                  <div
                    className={`max-w-[85%] md:max-w-[75%] p-4 rounded-2xl shadow-md transform transition-all duration-300 ${
                      msg.new ? (msg.sender === "user" ? "translate-x-4" : "-translate-x-4") : ""
                    } ${
                      msg.sender === "user"
                        ? `${
                            theme === "light"
                              ? "bg-gradient-to-r from-blue-500 to-blue-600"
                              : "bg-gradient-to-r from-blue-600 to-blue-800"
                          } 
                           text-white rounded-br-none`
                        : msg.sender === "system"
                          ? `${
                              theme === "light"
                                ? "bg-gradient-to-r from-gray-200 to-gray-300"
                                : "bg-gradient-to-r from-gray-700 to-gray-800"
                            } 
                             ${theme === "light" ? "text-gray-800" : "text-gray-200"}`
                          : `${
                              theme === "light"
                                ? "bg-gradient-to-r from-blue-50 to-blue-100"
                                : "bg-gradient-to-r from-blue-900/30 to-blue-800/30"
                            } 
                             ${theme === "light" ? "text-gray-800" : "text-gray-200"} rounded-bl-none`
                    }`}
                    style={{
                      boxShadow:
                        theme === "light" ? "0 4px 14px rgba(59, 130, 246, 0.15)" : "0 4px 14px rgba(30, 58, 138, 0.3)",
                      backdropFilter: "blur(10px)",
                    }}
                  >
                    {msg.isTyping ? (
                      <div>
                        {msg.text}
                        <span className="inline-block w-2 h-4 ml-1 bg-current animate-[blink_0.7s_step-end_infinite]"></span>
                      </div>
                    ) : msg.parts && msg.parts.length > 0 ? (
                      <div className="space-y-4">
                        {msg.parts.map((part, index) => (
                          <div key={index}>
                            {part.type === "text" && <div>{part.content}</div>}
                            {part.type === "code" && (
                              <CodeBlock
                                language={part.language}
                                code={part.code}
                                explanation={part.explanation}
                              />
                            )}
                          </div>
                        ))}
                      </div>
                    ) : (
                      msg.text
                    )}
                  </div>

                  {msg.sender === "user" && (
                    <div className="w-8 h-8 rounded-full bg-gradient-to-r from-indigo-400 to-indigo-600 flex items-center justify-center ml-2 flex-shrink-0 shadow-lg">
                      <svg
                        xmlns="http://www.w3.org/2000/svg"
                        className="h-5 w-5 text-white"
                        fill="none"
                        viewBox="0 0 24 24"
                        stroke="currentColor"
                      >
                        <path
                          strokeLinecap="round"
                          strokeLinejoin="round"
                          strokeWidth={2}
                          d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z"
                        />
                      </svg>
                    </div>
                  )}
                </div>
              ))}
              <div ref={messagesEndRef} />
            </div>
          )}
        </div>

        {/* Input Area */}
        <div
          className={`border-t p-4 ${theme === "light" ? 
            "border-blue-100 bg-white/80" : "border-blue-800 bg-blue-900/30"} 
            backdrop-blur-md rounded-sm mx-auto w-full max-w-2xl`}
          >
          <div className="max-w-3xl mx-auto">
            {/* File Upload Area */}
            {selectedFile && (
              <div
                className={`mb-3 p-3 rounded-lg flex items-center justify-between ${
                  theme === "light" ? "bg-blue-50" : "bg-blue-800/30"
                } animate-[fadeIn_0.3s_ease-in-out]`}
              >
                <div className="flex items-center">
                  <div className="w-8 h-8 rounded-full bg-gradient-to-r from-green-400 to-green-600 flex items-center justify-center mr-2">
                    <svg
                      xmlns="http://www.w3.org/2000/svg"
                      className="h-5 w-5 text-white"
                      fill="none"
                      viewBox="0 0 24 24"
                      stroke="currentColor"
                    >
                      <path
                        strokeLinecap="round"
                        strokeLinejoin="round"
                        strokeWidth={2}
                        d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"
                      />
                    </svg>
                  </div>
                  <span
                    className={`text-sm ${theme === "light" ? "text-gray-700" : "text-gray-300"} truncate max-w-xs`}
                  >
                    {selectedFile.name}
                  </span>
                </div>
                <button
                  onClick={() => setSelectedFile(null)}
                  className={`${theme === "light" ? "text-gray-500 hover:text-gray-700" : "text-gray-400 hover:text-gray-200"} transition-colors duration-300`}
                >
                  <svg
                    xmlns="http://www.w3.org/2000/svg"
                    className="h-5 w-5"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor"
                  >
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                  </svg>
                </button>
              </div>
            )}

            {/* Optional Filename Input */}
            <div className="mb-3">
              <input
                type="text"
                placeholder="Optional: File name"
                className={`w-full p-3 border rounded-lg focus:outline-none focus:ring-2 transition-all duration-300 ${
                  theme === "light"
                    ? "border-black-200 focus:ring-black-500 focus:border-transparent bg-white"
                    : "border-blue-700 focus:ring-black-400 focus:border-black bg-blue-800/30 text-white placeholder-blue-300"
                }`}
                value={fileName}
                onChange={(e) => setFileName(e.target.value)}
              />
            </div>

            {/* Main Input Area */}
            <div className="flex flex-col md:flex-row space-y-3 md:space-y-0 md:space-x-3">
              <div className="flex-1 relative">
                <textarea
                  placeholder="Type your message here..."
                  className={`w-full p-4 pr-12 border rounded-lg resize-none h-[100px] md:h-[60px] focus:outline-none focus:ring-2 transition-all duration-300 ${
                    theme === "light"
                      ? "border-blue-200 focus:ring-blue-500 focus:border-transparent bg-white"
                      : "border-blue-700 focus:ring-blue-400 focus:border-transparent bg-blue-800/30 text-white placeholder-blue-300"
                  }`}
                  style={{
                    boxShadow: isSending
                      ? theme === "light"
                        ? "0 0 0 3px rgba(59, 130, 246, 0.3)"
                        : "0 0 0 3px rgba(37, 99, 235, 0.3)"
                      : "none",
                    animation: isSending ? "pulse-border 2s infinite" : "none",
                  }}
                  value={prompt}
                  onChange={(e) => setPrompt(e.target.value)}
                  onKeyDown={handleKeyDown}
                  rows={1}
                />
                <label
                  htmlFor="file-upload"
                  className={`absolute right-3 bottom-3 cursor-pointer transition-all duration-300 transform hover:scale-110 ${
                    theme === "light" ? "text-blue-600 hover:text-blue-800" : "text-blue-300 hover:text-blue-100"
                  }`}
                >
                  <svg
                    xmlns="http://www.w3.org/2000/svg"
                    className="h-6 w-6"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor"
                  >
                    <path
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      strokeWidth={2}
                      d="M15.172 7l-6.586 6.586a2 2 0 102.828 2.828l6.414-6.586a4 4 0 00-5.656-5.656l-6.415 6.585a6 6 0 108.486 8.486L20.5 13"
                    />
                  </svg>
                  <input
                    id="file-upload"
                    type="file"
                    className="hidden"
                    onChange={(e) => setSelectedFile(e.target.files[0])}
                  />
                </label>
              </div>

              <div className="flex space-x-2">
                {selectedFile && (
                  <button
                    className={`px-4 py-3 rounded-lg font-medium transition-all duration-300 transform hover:scale-105 ${
                      isUploading
                        ? theme === "light"
                          ? "bg-green-200 text-green-700 cursor-not-allowed"
                          : "bg-green-800/50 text-green-300 cursor-not-allowed"
                        : theme === "light"
                          ? "bg-gradient-to-r from-green-500 to-green-600 text-white hover:from-green-600 hover:to-green-700 shadow-md hover:shadow-lg"
                          : "bg-gradient-to-r from-green-600 to-green-700 text-white hover:from-green-700 hover:to-green-800 shadow-md hover:shadow-lg"
                    }`}
                    onClick={handleFileUpload}
                    disabled={isUploading}
                    style={{
                      boxShadow:
                        theme === "light" ? "0 4px 14px rgba(74, 222, 128, 0.4)" : "0 4px 14px rgba(22, 163, 74, 0.4)",
                    }}
                  >
                    {isUploading ? (
                      <div className="flex items-center">
                        <svg
                          className="animate-spin -ml-1 mr-2 h-4 w-4 text-current"
                          xmlns="http://www.w3.org/2000/svg"
                          fill="none"
                          viewBox="0 0 24 24"
                        >
                          <circle
                            className="opacity-25"
                            cx="12"
                            cy="12"
                            r="10"
                            stroke="currentColor"
                            strokeWidth="4"
                          ></circle>
                          <path
                            className="opacity-75"
                            fill="currentColor"
                            d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
                          ></path>
                        </svg>
                        Uploading...
                      </div>
                    ) : (
                      "Upload"
                    )}
                  </button>
                )}

                <button
                  className={`px-6 py-3 rounded-lg font-medium transition-all duration-300 transform hover:scale-105 ${
                    isSending || !prompt.trim()
                      ? theme === "light"
                        ? "bg-blue-300 cursor-not-allowed"
                        : "bg-blue-700/50 text-blue-300 cursor-not-allowed"
                      : theme === "light"
                        ? "bg-gradient-to-r from-blue-500 to-blue-600 text-white hover:from-blue-600 hover:to-blue-700 shadow-md hover:shadow-lg"
                        : "bg-gradient-to-r from-blue-600 to-blue-700 text-white hover:from-blue-700 hover:to-blue-800 shadow-md hover:shadow-lg"
                  }`}
                  onClick={handleSend}
                  disabled={isSending || !prompt.trim()}
                  style={{
                    boxShadow:
                      theme === "light" ? "0 4px 14px rgba(59, 130, 246, 0.4)" : "0 4px 14px rgba(37, 99, 235, 0.4)",
                  }}
                >
                  {isSending ? (
                    <div className="flex items-center">
                      <svg
                        className="animate-spin -ml-1 mr-2 h-4 w-4 text-white"
                        xmlns="http://www.w3.org/2000/svg"
                        fill="none"
                        viewBox="0 0 24 24"
                      >
                        <circle
                          className="opacity-25"
                          cx="12"
                          cy="12"
                          r="10"
                          stroke="currentColor"
                          strokeWidth="4"
                        ></circle>
                        <path
                          className="opacity-75"
                          fill="currentColor"
                          d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
                        ></path>
                      </svg>
                      Sending...
                    </div>
                  ) : (
                    "Send"
                  )}
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}
