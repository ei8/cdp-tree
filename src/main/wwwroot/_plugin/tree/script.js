﻿var BlazorScrollToId = function (id) {
    debugger;
    const element = document.getElementById(id);
    if (element instanceof HTMLElement) {
        element.scrollIntoView({
            behavior: "smooth",
            block: "start",
            inline: "nearest"
        });
    }
}

function copyToClipboard(text) {
    var input = document.body.appendChild(document.createElement("input"));
    input.value = text;
    input.focus();
    input.select();
    document.execCommand('copy');
    input.parentNode.removeChild(input);
}

function displayGraph(nodes, links) {
    var width = 500,
        height = 500;
    var container = null,
        path = null,
        node = null,
        text = null,
        currentDraggingNode = null;
    var domNode = null;
    var selectedNode = null;
    // register events after 2 seconds
    setTimeout(function () {
        document.getElementById('contextmenu').addEventListener("click", selectNode);
        const overlay = document.getElementsByClassName('svgview')[0];
        overlay.addEventListener('click', () => {
            const modals = document.querySelectorAll('.modal.active')

            modals.forEach(modal => {
                closeModal(modal)
            })
        });
    }, 2000);
    function update() {
        var force = d3.layout.force()
            .nodes(d3.values(nodes))
            .links(links)
            .size([width, height])
            .linkDistance(60)
            .charge(-300)
            .on("tick", tick)
            .start();

        var zoom = d3.behavior.zoom()
            .scaleExtent([1, 10])
            .on("zoom", zoomed);

        var drag = force.drag()
            .on("dragstart", dragStarted);

        d3.selectAll("svg > *").remove();

        var svg = d3.select("svg")
            .attr("width", width)
            .attr("height", height)
            .call(zoom);

        // Per-type markers, as they don't inherit styles.
        svg.append("defs").selectAll("marker")
            .data(["fullexcite", "fullinhibit", "partialexcite", "partialinhibit"])
            .enter().append("marker")
            .attr("id", function (d) { return d; })
            .attr("viewBox", "0 -5 10 10")
            .attr("refX", 15)
            .attr("refY", -1.5)
            .attr("markerWidth", 6)
            .attr("markerHeight", 6)
            .attr("orient", "auto")
            .append("path")
            .attr("d", "M0,-5L10,0L0,5")
            .attr("fill", function (d) { if (d.includes("inhibit")) { return "#f00"; } else { return "#000"; } });
        svg.on("click", selectEnded);
        container = svg.append("g");

        path = container.append("g").selectAll("path")
            .data(force.links())
            .enter().append("path")
            .attr("class", function (d) { return "link " + d.type; })
            .attr("marker-end", function (d) { return "url(#" + d.type + ")"; });

        // Define linkText variable
        var linkText = container.append("g").selectAll("text")
            .data(force.links())
            .enter().append("text")
            .attr("class", "link-label")
            .attr("dy", -5) // Adjust the position above the line
            .text(function(d) { 
                if (d.Strength < 1 && d.type.includes("excite")) {
                    return d.Strength;
                } else if (d.type.includes("inhibit")) {
                    return -d.Strength;
                } else {
                    return '';
                }
            });

        node = container.append("g")
            .attr("class", "nodes")
            .selectAll(".node")
            .data(force.nodes())
            .enter().append("g")
            .attr("class", "node")
            .attr("onmouseover", "hover('div', this.id, true, highlightTreeItem)")
            .attr("onmouseout", "hover('div', this.id, false, highlightTreeItem)")
            .attr("id", d => d.id)
            .attr("cx", function (d) { return d.x; })
            .attr("cy", function (d) { return d.y; })
            .call(drag)
            .on("contextmenu", startSelect);

        node.append("circle")
            .attr("r", 6)
            .on("dblclick", dblclick);

        node.append("circle")
            .attr('class', 'ghostCircle')
            .attr("r", 25)
            .attr("opacity", 0.3) // change this to zero to hide the target area
            .style("fill", "red")
            .on("mouseover", function (node) {
                overCircle(node);
            })
            .on("mouseout", function (node) {
                outCircle(node);
            });
        text = container.append("g").selectAll("text")
            .data(force.nodes())
            .enter()
            .append("text")
            .attr("x", -7)
            .attr("y", 7);

        text.selectAll("tspan.text")
            .data(d => (d.tag || '').split("\n") || "")
            .enter()
            .append("tspan")
            .attr("class", "text")
            .text(d => d.replace(/ /g, '\u00a0'))
            .attr("x", -7)
            .attr("dx", 0)
            .attr("dy", 10);
    }
    update();
    // Use elliptical arc path segments to doubly-encode directionality.
    function tick() {
        path.attr("d", linkArc);
        node.attr("transform", transform);
        text.attr("transform", transform);

        // Update the position of the text
        container.selectAll(".link-label").attr("transform", function(d) {
            return "translate(" + ((d.source.x + d.target.x) / 2) + "," + ((d.source.y + d.target.y) / 2) + ")";
        });
    }

    function linkArc(d) {

        var x0 = d.source.x;
        var y0 = d.source.y;
        var x1 = d.target.x;
        var y1 = d.target.y;
        var xcontrol = x1 * 0.5 + x0 * 0.5;
        return ["M", x0, y0, "C", xcontrol, y0, xcontrol, y1, x1, y1].join(" ");
        //var dx = d.target.x - d.source.x,
        //    dy = d.target.y - d.source.y,
        //    dr = Math.sqrt(dx * dx + dy * dy);
        //return "M" + d.source.x + "," + d.source.y + "A" + dr + "," + dr + " 0 0,1 " + d.target.x + "," + d.target.y;
    }

    function transform(d) {
        return "translate(" + d.x + "," + d.y + ")";
    }

    function initiateDrag(d, domNode) {
        draggingNode = d;
        d3.select(domNode).select('.ghostCircle').attr('pointer-events', 'none');
        d3.selectAll('.ghostCircle').attr('class', 'ghostCircle show');
        d3.select(domNode).attr('class', 'node activeDrag');
        node.selectAll("g.node").sort(function (a, b) { // select the parent and sort the path's
            if (a.id != draggingNode.id) return 1; // a is not the hovered element, send "a" to the back
            else return -1; // a is the hovered element, bring "a" to the front
        });

        if (nodes.length > 1) {
            nodePaths = node.selectAll("path.link")
                .data(links, function (d) {
                    return d.target.id;
                }).remove();
            nodesExit = node.selectAll("g.node")
                .data(nodes, function (d) {
                    return d.id;
                }).filter(function (d, i) {
                    if (d.id == draggingNode.id) {
                        return false;
                    }
                    return true;
                }).remove();
        }
    }

    var updateTempConnector = function () {
        var data = [];
        if (draggingNode !== null && selectedNode !== null) {
            // have to flip the source coordinates since we did this for the existing connectors on the original tree
            data = [{
                source: {
                    x: draggingNode.x,
                    y: draggingNode.y
                },
                target: {
                    x: selectedNode.x,
                    y: selectedNode.y
                }
            }];
        }

        d3.selectAll(".templink").remove();

        var link = node.select(".templink").data(data);
        link.exit().remove();

        container.append("path")
            .data(data)
            .attr("class", "templink")
            .attr("d", d3.svg.diagonal())
            .attr('pointer-events', 'none');
    };

    function startSelect(d) {
        d.fixed = true;
        d.fx = d.x;
        d.fy = d.y;
        d3.event.preventDefault();

        const modal = document.getElementById("modal");

        openModal(modal, d)
        currentDraggingNode = d;
    }

    function selectNode(d) {

        currentNode = d;
        const modal = document.getElementById("modal");
        closeModal(modal)
        d = currentDraggingNode;

        d3.selectAll("ghostCircle");

        domNode = document.getElementById(d.id);
        initiateDrag(d, domNode);
        updateTempConnector();
        tick();

    }

    async function selectEnded(d) {
        if (d == undefined) {
            d = currentNode;
        }
        d.fx = d.x;
        d.fy = d.y;
        d3.selectAll('.ghostCircle').attr('class', 'ghostCircle');

        updateTempConnector();
        // now restore the mouseover event or we won't be able to drag a 2nd time
        d3.select(domNode).select('.ghostCircle').attr('pointer-events', '');
        domNode = this;

        if (selectedNode) {
            var addlink = { "source": nodes[nodes.indexOf(draggingNode)], "target": nodes[nodes.indexOf(selectedNode)], "value": 1, "index": 1, "type": "fullexcite" }

            // now remove the element from the parent, and insert it into the new elements children

            var progress = document.getElementsByClassName("progress")[0];
            progress.classList.remove('none');
            d.fixed = true;
            // of course set the node to fixed so the force doesn't include the node in its auto positioning stuff

            var status = await dotNetHelper.invokeMethodAsync('SendLinkData', draggingNode.id, selectedNode.id);

            if (status) {
                links.push(addlink);
            }
            updateTempConnector();
            update();

            selectedNode = null;
            draggingNode = null;
            progress.classList.add('none');
        }
    }

    function dragStarted(d) {
        d.fixed = true;
        d3.event.sourceEvent.stopPropagation();
    }

    var overCircle = function (d) {
        selectedNode = d;
        updateTempConnector();
    };

    var outCircle = function (d) {
        d3.selectAll(".templink").remove();
        selectedNode = null;
        updateTempConnector();
    };

    function dblclick(d) {
        event.stopPropagation();
        d.fixed = false;
    }

    function zoomed() {
        container.attr("transform", "translate(" + d3.event.translate + ")scale(" + d3.event.scale + ")");
    }

    function openModal(modal, currentNode) {

        var body = document.getElementsByClassName("modal-list")[0];

        if (modal == null) return;

        modal.classList.add('active');

        var x = d3.event.pageX;
        var y = d3.event.pageY;
        modal.style.left = "" + x + "px";
        modal.style.top = "" + y + "px";
    }

    function closeModal(modal) {
        if (modal == null) return
        modal.classList.remove('active')
        overlay.classList.remove('active')
    }
}

function highlightTreeItem(currElement, over) {
    //get attribute value and set hover color
    var element = currElement.getAttribute("userCreated");

    // TODO: yellow, blue, and gray come from _host.cshtml, should use class in css instead?
    if (element == "true") {
        currElement.style.backgroundColor = over ? yellow : blue;
    } else {
        currElement.style.backgroundColor = over ? blue : gray;
    }
}

// tree
window.PlaySound = function () {
    document.getElementById('sound').play();
}
