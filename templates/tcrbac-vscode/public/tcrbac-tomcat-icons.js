(() => {
  const iconClass = "tomcat-inline-icon";
  const iconSource = "https://tomcat.apache.org/res/images/tomcat.png";
  const skipTags = new Set(["SCRIPT", "STYLE", "CODE", "PRE", "KBD", "SAMP", "TITLE", "TEXTAREA"]);
  const wordPattern = /\bTomcat('s)?\b/g;

  function createIcon() {
    const icon = document.createElement("img");
    icon.className = iconClass;
    icon.src = iconSource;
    icon.alt = "Apache Tomcat";
    icon.title = "Apache Tomcat";
    icon.loading = "lazy";
    icon.decoding = "async";
    return icon;
  }

  function shouldSkip(node) {
    for (let element = node.parentElement; element; element = element.parentElement) {
      if (skipTags.has(element.tagName) || element.classList.contains(iconClass)) {
        return true;
      }
    }

    return false;
  }

  function replaceTomcatText(node) {
    const text = node.nodeValue;
    if (!wordPattern.test(text) || shouldSkip(node)) {
      wordPattern.lastIndex = 0;
      return;
    }

    wordPattern.lastIndex = 0;
    const fragment = document.createDocumentFragment();
    let lastIndex = 0;
    let match;

    while ((match = wordPattern.exec(text)) !== null) {
      fragment.append(document.createTextNode(text.slice(lastIndex, wordPattern.lastIndex)));
      fragment.append(createIcon());
      lastIndex = wordPattern.lastIndex;
    }

    fragment.append(document.createTextNode(text.slice(lastIndex)));
    node.replaceWith(fragment);
  }

  function addTomcatIcons() {
    if (document.documentElement.dataset.tcrbacTomcatIcons === "true") {
      return;
    }

    document.documentElement.dataset.tcrbacTomcatIcons = "true";
    const root = document.querySelector("article") || document.querySelector("main") || document.body;
    const walker = document.createTreeWalker(root, NodeFilter.SHOW_TEXT);
    const nodes = [];

    while (walker.nextNode()) {
      nodes.push(walker.currentNode);
    }

    nodes.forEach(replaceTomcatText);
  }

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", addTomcatIcons, { once: true });
  } else {
    addTomcatIcons();
  }
})();
