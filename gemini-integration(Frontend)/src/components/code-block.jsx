"use client"

import { useState } from "react"
import { Check, Copy } from "lucide-react"
import { Prism as SyntaxHighlighter } from "react-syntax-highlighter"
import { oneDark } from "react-syntax-highlighter/dist/esm/styles/prism"
import ReactMarkdown from "react-markdown"
import remarkGfm from "remark-gfm"

export function CodeBlock({ language, code, explanation }) {
  const [copied, setCopied] = useState(false)

  const copyToClipboard = async () => {
    try {
      await navigator.clipboard.writeText(code)
      setCopied(true)
      setTimeout(() => setCopied(false), 2000)
    } catch (err) {
      console.error("Failed to copy text: ", err)
    }
  }

  return (
    <div className="my-4 rounded-md overflow-hidden border border-gray-200 dark:border-gray-800 shadow-sm">
      {/* Header with language and Copy button */}
      <div className="bg-gray-100 dark:bg-gray-800 p-3 flex justify-between items-center">
        <span className="text-sm font-mono text-gray-700 dark:text-gray-300">{language}</span>
        <button
          onClick={copyToClipboard}
          className="text-xs flex items-center gap-1 text-gray-600 dark:text-gray-400 hover:text-gray-900 dark:hover:text-white transition-colors"
        >
          {copied ? (
            <>
              <Check className="h-3.5 w-3.5" />
              <span>Copied!</span>
            </>
          ) : (
            <>
              <Copy className="h-3.5 w-3.5" />
              <span>Copy code</span>
            </>
          )}
        </button>
      </div>

      {/* Code block */}
      <SyntaxHighlighter
        language={language}
        style={oneDark}
        customStyle={{
          margin: 0,
          padding: "1rem",
          borderRadius: 0,
        }}
      >
        {code}
      </SyntaxHighlighter>

      {/* Explanation section with Markdown support */}
      {explanation && (
        <div className="p-4 bg-gray-50 dark:bg-gray-900 text-gray-800 dark:text-gray-200 border-t border-gray-200 dark:border-gray-800">
          <h3 className="font-medium mb-2 text-gray-900 dark:text-white">Explanation:</h3>
          <div className="markdown-content">
            <ReactMarkdown
              remarkPlugins={[remarkGfm]}  // Enables GitHub Flavored Markdown (GFM)
              components={{
                p: ({ node, ...props }) => <p className="mb-3" {...props} />, // paragraph styling
                strong: ({ node, ...props }) => (
                  <strong className="font-semibold text-gray-900 dark:text-white" {...props} />
                ),
                ol: ({ node, ...props }) => <ol className="list-decimal pl-5 mb-4 space-y-2" {...props} />, // ordered list
                ul: ({ node, ...props }) => <ul className="list-disc pl-5 mb-4 space-y-1" {...props} />, // unordered list
                li: ({ node, ...props }) => <li className="mb-1" {...props} />, // list items
                code: ({ node, inline, ...props }) =>
                  inline ? (
                    <code className="px-1 py-0.5 bg-gray-200 dark:bg-gray-800 rounded text-sm font-mono" {...props} />
                  ) : (
                    <code {...props} />
                  ),
                h1: ({ node, ...props }) => <h1 className="text-xl font-bold mt-6 mb-3" {...props} />,
                h2: ({ node, ...props }) => <h2 className="text-lg font-bold mt-5 mb-2" {...props} />,
                h3: ({ node, ...props }) => <h3 className="text-md font-bold mt-4 mb-2" {...props} />,
              }}
            >
              {explanation}
            </ReactMarkdown>
          </div>
        </div>
      )}
    </div>
  )
}